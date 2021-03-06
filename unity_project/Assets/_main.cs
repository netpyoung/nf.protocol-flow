﻿using System;
using AutoGenerated.Message;
using NF.Network.Transfer.Protobuf;
using NF.Results;
using UnityEngine;

namespace Game
{
    public class _main : MonoBehaviour
    {
        private async void Start()
        {
            ProtobufHttpSender sender = new ProtobufHttpSender(new Uri("http://127.0.0.1:8080/"), 3000);
            Result<RHello, int> t = await sender.Hello(1, 2);
            if (t.IsErr)
            {
                Debug.LogError(t.Err);
                return;
            }

            Debug.Log(t.Ok);
        }
    }
}