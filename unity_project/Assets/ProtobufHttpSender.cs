﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoGenerated.Transfer;
using Google.Protobuf;
using NF.Results;
using UnityEngine.Networking;

namespace NF.Network.Transfer.Protobuf
{
    public class ProtobufHttpSender : BaseSender
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        public readonly Uri Uri;

        public ProtobufHttpSender(Uri uri, int timeoutMilliseconds)
        {
            this.Uri = uri;
            this.TimeoutMilliseconds = timeoutMilliseconds;
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.Expect100Continue = false;
        }

        public int TimeoutMilliseconds { get; }

        public override async Task<Result<R, int>> Send<R>(IMessage reqMessage)
        {
            try
            {
                UnityWebRequest webRequest = new UnityWebRequest(this.Uri);
                webRequest.method = "POST";
                webRequest.timeout = this.TimeoutMilliseconds / 1000;

                byte[] reqBytes = reqMessage.ToByteArray();

                UploadHandlerRaw uploadHandler = new UploadHandlerRaw(reqBytes);
                uploadHandler.contentType = "application/json";
                webRequest.uploadHandler = uploadHandler;
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                UnityWebRequestAsyncOperation x = webRequest.SendWebRequest();
                if (!x.isDone)
                {
                    await Task.Delay(200);
                }


                if (webRequest.isHttpError)
                {
                    return Result.Err<R, int>(-4);
                }

                if (webRequest.isNetworkError)
                {
                    return Result.Err<R, int>(-3);
                }

                byte[] bytes = webRequest.downloadHandler.data;
                R ret = new R();
                ret.MergeFrom(bytes);
                return ret.ToOk<R, int>();
            }
            catch (InvalidProtocolBufferException e)
            {
                UnityEngine.Debug.LogException(e);
                return Result.Err<R, int>(-2);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return Result.Err<R, int>(-1);
            }
        }

        public async Task<Result<R, int>> HttpSend<R>(IMessage reqMessage) where R : IMessage, new()
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.Uri);
                httpWebRequest.Method = "POST";
                httpWebRequest.CookieContainer = this._cookieContainer;
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Timeout = this.TimeoutMilliseconds;

                byte[] reqBytes = reqMessage.ToByteArray();
                using (Stream stream = await httpWebRequest.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(reqBytes, 0, reqBytes.Length);
                    await stream.FlushAsync();
                }

                using (WebResponse response = await httpWebRequest.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        R ret = new R();
                        ret.MergeFrom(stream);
                        return ret.ToOk<R, int>();
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return Result.Err<R, int>(-1);
            }
        }
    }
}