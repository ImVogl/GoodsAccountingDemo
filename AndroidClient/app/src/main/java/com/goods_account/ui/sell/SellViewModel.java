package com.goods_account.ui.sell;

import android.arch.lifecycle.ViewModel;
import java.util.function.Supplier;
import androidx.databinding.ObservableBoolean;
import androidx.databinding.ObservableInt;
import com.goods_account.api.ApiClient;
import com.goods_account.api.models.SellType;
import com.goods_account.data.model.Settings;

import static android.view.View.VISIBLE;
import static android.view.View.INVISIBLE;

/**
 * View model for "sell screen".
 */
public class SellViewModel extends ViewModel {

    /**
     * Start scanning button visible state.
     */
    public ObservableInt start_scan_visible = new ObservableInt(VISIBLE);

    /**
     * This field is indicating that start scanning button is enabled.
     */
    public ObservableBoolean enabled_start_scan = new ObservableBoolean(true);

    /**
     * Scanning buttons visible state.
     */
    public ObservableInt realise_scan_visible = new ObservableInt(INVISIBLE);

    /**
     * This field is indicating that scanning buttons is enabled.
     */
    public ObservableBoolean enabled_realise_scan = new ObservableBoolean(false);

    /**
     * Scanning buttons visible state.
     */
    public ObservableInt selling_visible = new ObservableInt(INVISIBLE);

    /**
     * This field is indicating that scanning buttons is enabled.
     */
    public ObservableBoolean selling_scan = new ObservableBoolean(false);

    /**
     * Current barcode data.
     */
    private String _data;

    /**
     * Api client instance;
     */
    private ApiClient _client;

    /**
     * Update scanner screen.
     */
    private Supplier<Boolean> _updateScanner;

    /**
     * Initializing new instance.
     */
    public SellViewModel(){
        Settings settings = Settings.getInstance();
        _client = new ApiClient(settings.getApiServiceUrl(), settings.getPathToCertificate(), settings.getPathToSecretKey());
    }

    /**
     * Initializing ViewModel.
     * @param updateScanner Action for update scanner screen.
     */
    public void initialize(Supplier<Boolean> updateScanner){
        _updateScanner = updateScanner;
    }

    /**
     * Set view start scanning state.
     */
    public void startScanning(){
        start_scan_visible.set(INVISIBLE);
        enabled_start_scan.set(false);

        realise_scan_visible.set(VISIBLE);
        enabled_realise_scan.set(true);
        selling_visible.set(INVISIBLE);
        selling_scan.set(false);
        _updateScanner.get();
    }

    /**
     * Set view release scanning state.
     */
    public void stopScanning(){
        start_scan_visible.set(VISIBLE);
        enabled_start_scan.set(true);
        realise_scan_visible.set(INVISIBLE);
        enabled_realise_scan.set(false);
        selling_visible.set(INVISIBLE);
        selling_scan.set(false);
    }

    /**
     * Setting data from barcode.
     * @param data Data from barcode.
     */
    public void setScanned(String data){
        selling_visible.set(VISIBLE);
        selling_scan.set(true);
        enabled_realise_scan.set(false);
        _data = data;
    }

    /**
     * Selling item with cash.
     */
    public void sellCash(){
        _client.sell(1, _data, SellType.CASH);
        startScanning();
    }

    /**
     * Selling item with credit card.
     */
    public void sellCreditCard(){
        _client.sell(1, _data, SellType.CREDIT_CARD);
        startScanning();
    }

    /**
     * Selling item with whole scale price.
     */
    public void sellWholePrice(){
        _client.sell(1, _data, SellType.WHOLE_SCALE_PRICE);
        startScanning();
    }
}
