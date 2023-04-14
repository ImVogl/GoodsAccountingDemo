package com.goods_account_service;

// This exception is throwing when server returned non 200 status code.
public class NativeClientRequestException extends Exception{
    public NativeClientRequestException(int statusCode){
        super("Server returned response with unsuccessful status code (" + statusCode + ")");
    }

    public NativeClientRequestException(){
        super("Server returned response with unsuccessful status code");
    }
}