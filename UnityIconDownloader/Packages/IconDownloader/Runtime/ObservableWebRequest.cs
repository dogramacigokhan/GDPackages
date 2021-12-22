using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace IconDownloader
{
    public static class ObservableWebRequest
    {

        public static IObservable<string> GetString(
            string url,
            Dictionary<string, string> headers = null)
        {
            return Download(
                    url,
                    UnityWebRequest.Get,
                    headers)
                .Select(request => request.downloadHandler.text);
        }

        public static IObservable<T> GetObject<T>(
            string url,
            Dictionary<string, string> headers = null)
        {
            return GetString(url, headers)
                .Select(JsonConvert.DeserializeObject<T>);
        }

        public static IObservable<T> PostObject<T>(
            string url,
            string data,
            Dictionary<string, string> headers = null)
        {
            return Download(
                    url,
                    uri => new UnityWebRequest(
                        uri,
                        UnityWebRequest.kHttpVerbPOST,
                        new DownloadHandlerBuffer(),
                        new UploadHandlerRaw(Encoding.UTF8.GetBytes(data))),
                    headers)
                .Select(request => request.downloadHandler.text)
                .Select(JsonConvert.DeserializeObject<T>);
        }

        public static IObservable<Texture2D> GetTexture(
            string url,
            Dictionary<string, string> headers = null)
        {
            return Download(
                    url,
                    u => UnityWebRequestTexture.GetTexture(u, nonReadable: false),
                    headers)
                .Select(DownloadHandlerTexture.GetContent);
        }

        private static IObservable<UnityWebRequest> Download(
            string url,
            Func<string, UnityWebRequest> webRequestFunc,
            Dictionary<string, string> headers = null)
        {
            return Observable.Create<UnityWebRequest>(observer =>
            {
                var request = webRequestFunc(url);
                var requestDisposable = new SingleAssignmentDisposable();
                
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }

                // Set default header to accept json
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "application/json");

                IObservable<UnityWebRequest> HandleResult()
                {
                    if (requestDisposable.IsDisposed)
                    {
                        return Observable.Throw<UnityWebRequest>(
                            new OperationCanceledException("Already disposed."));
                    }
                    
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        return Observable.Throw<UnityWebRequest>(new WebException(request.error));
                    }

                    if (request.responseCode != (long)HttpStatusCode.OK)
                    {
                        return Observable.Throw<UnityWebRequest>(
                            new WebException($"{request.responseCode} - {request.downloadHandler.text}"));
                    }

                    return Observable.Return(request);
                }


                var observableRequest = request
                    .SendWebRequest()
                    .AsObservable();

                if (!Application.isPlaying)
                {
                    // Observable web request is not waiting for their underlying request's completion in Editor.
                    // So we are continuing with a timer until it's completed.
                    observableRequest = observableRequest
                        .ContinueWith(operation => Observable
                            .Interval(TimeSpan.FromSeconds(0.1))
                            .SkipWhile(_ => !request.isDone)
                            .Select(_ => operation)
                            .Take(1));
                }

                requestDisposable.Disposable = observableRequest
                    .ContinueWith(_ => HandleResult())
                    .CatchIgnore((OperationCanceledException _) => observer.OnCompleted())
                    .Subscribe(result =>
                    {
                        observer.OnNext(result);
                        observer.OnCompleted();
                    });

                return new CompositeDisposable(
                    request,
                    requestDisposable);
            });
        }
    }
}