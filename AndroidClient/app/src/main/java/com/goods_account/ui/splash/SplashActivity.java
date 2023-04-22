package com.goods_account.ui.splash;

import android.content.Intent;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import com.goods_account.R;
import com.goods_account.api.ApiClient;
import com.goods_account.data.model.Settings;
import com.goods_account.ui.login.LoginActivity;
import com.goods_account.ui.sell.SellActivity;

import java.io.FileNotFoundException;

public class SplashActivity extends AppCompatActivity {
    /**
     * Client of api service.
     */
    private ApiClient _client;

    /**
     * Create new instance of SplashActivity.
     */
    public SplashActivity(){
        Settings settings = Settings.getInstance();
        _client = new ApiClient(settings.getApiServiceUrl(), settings.getPathToCertificate(), settings.getPathToSecretKey());
    }

    /**
     * @param savedInstanceState If the activity is being re-initialized after
     *                           previously being shut down then this Bundle contains the data it most
     *                           recently supplied in {@link #onSaveInstanceState}.  <b><i>Note: Otherwise it is null.</i></b>
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash_screen);

        String certificatePath = getResources().getString(R.string.certPath);
        String keyPath = getResources().getString(R.string.keyPath);
        String apiUrl = getResources().getString(R.string.apiUrl);
        Settings settings = Settings.getInstance();
        try {
            settings.initialize(certificatePath, keyPath, apiUrl);
        } catch (FileNotFoundException exception){
            this.finishAffinity();
        } catch (NullPointerException exception){
            this.finishAffinity();
        }

        Handler handler = new Handler();
        handler.postDelayed(() -> {
            if (_client.refreshToken()){
                startActivity(new Intent(SplashActivity.this, SellActivity.class));
            } else {
                startActivity(new Intent(SplashActivity.this, LoginActivity.class));
            }
        }, 4000);
    }
}