package com.goods_account.api.models;

import com.google.gson.annotations.SerializedName;
import java.util.HashMap;

/**
 * Sold item DTO.
 */
public class SoldItem {

    /**
    * Get or set user's identifier.
    */
    @SerializedName("id")
    public int Id;

    /**
    * Get or set dictionary where key: item guid; value: sold item count.
    */
    @SerializedName("sold")
    public HashMap<String, Integer> Sold;
}
