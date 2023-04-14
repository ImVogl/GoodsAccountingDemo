package com.goods_account_service;

import java.lang.IllegalArgumentException;
import java.util.IllegalFormatException;

import com.goods_account_service.NativeClientRequestException;

public class ClientResponse
{
    /**
     * Constructor.
     * @param exception - thrown exception.
     */
    public ClientResponse(Exception exception){
        _exception = exception;
    }

    /**
     * @param responseCode - response error code.
     */
    public ClientResponse(int responseCode){
        try {
            _response = String.format("{ \"ErrorCode\": %n }", responseCode);
        } catch (IllegalFormatException exception){
            _response = "{ \"ErrorCode\": -1 }";
        }

        try {
            _exception = new NativeClientRequestException(responseCode);
        } catch (IllegalArgumentException exception) {
            _exception = new NativeClientRequestException();
        }
    }

    /**
     * @param body - response body.
     */
    public ClientResponse(String body){
        _response = body;
        _exception = null;
    }

    /**
     * @return response trown exception.
     */
    public Exception get_exception() {
        return _exception;
    }

    /**
     * @return result response content.
     */
    public String get_response() {
        return _response;
    }

    /**
     * Trown exception.
     */
    private Exception _exception;

    // Content from response body or error code.
    private String _response;
}