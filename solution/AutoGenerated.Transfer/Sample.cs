﻿/***************************
 * DO NOT MODIFY THIS FILE
 ***************************/

/*****************
 * THIS IS SAMPLE
 *****************/
/*
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoGenerated.Transfer;
using Google.Protobuf;
using NF.Results;

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
                HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(this.Uri);
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
*/