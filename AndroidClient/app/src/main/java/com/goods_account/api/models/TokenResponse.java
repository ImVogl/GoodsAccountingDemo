package com.goods_account.api.models;

import com.google.gson.annotations.SerializedName;

/**
 * Model of body for response with token.
 */
public class TokenResponse {

    /**
     * Get or set access token.
     */
    @SerializedName("token")
    public String Token;
}
