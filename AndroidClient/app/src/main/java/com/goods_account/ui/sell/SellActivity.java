package com.goods_account.ui.sell;

import android.arch.lifecycle.ViewModelProvider;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageButton;
import android.widget.Toast;
import com.budiyev.android.codescanner.AutoFocusMode;
import com.budiyev.android.codescanner.CodeScanner;
import com.budiyev.android.codescanner.ScanMode;
import com.goods_account.R;
import com.goods_account.databinding.ActivitySellBinding;
import com.goods_account.ui.ViewModelFactory;
import com.budiyev.android.codescanner.CodeScannerView;
import com.google.zxing.BarcodeFormat;

import java.util.ArrayList;

/**
 * Selling screen activity.
 * https://github.com/yuriy-budiyev/code-scanner
 */
public class SellActivity extends AppCompatActivity  {

    /**
     * Scanner of QR codes.
     */
    private CodeScanner _codeScanner;

    /**
     * Binding instance for "sell screen".
     */
    private ActivitySellBinding _binding;

    /**
     * ViewModel for this activity.
     */
    private SellViewModel _viewModel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sell);

        _binding = ActivitySellBinding.inflate(getLayoutInflater());
        setContentView(_binding.getRoot());
        initializeCamera();
        _viewModel = new ViewModelProvider(this, new ViewModelFactory()).get(SellViewModel.class);
    }

    /**
     * Initialization of camera.
     */
    private void initializeCamera(){
        CodeScannerView scannerView = _binding.scanner;
        _codeScanner = new CodeScanner(this, scannerView);
        _viewModel.initialize(() -> { _codeScanner.startPreview(); return true; }); // scannerView.setOnClickListener(view -> _codeScanner.startPreview());

        // Parameters (default values)
        // or CAMERA_FRONT or specific camera id
        _codeScanner.setCamera(CodeScanner.CAMERA_BACK);

        // list of type BarcodeFormat
        ArrayList<BarcodeFormat> formats = new ArrayList<BarcodeFormat>();
        formats.add(BarcodeFormat.CODE_128);
        formats.add(BarcodeFormat.QR_CODE);
        _codeScanner.setFormats(formats);

        _codeScanner.setAutoFocusMode(AutoFocusMode.SAFE);  // or CONTINUOUS
        _codeScanner.setScanMode(ScanMode.SINGLE);          // or CONTINUOUS or PREVIEW
        _codeScanner.setAutoFocusEnabled(true);             // Whether to enable auto focus or not
        _codeScanner.setFlashEnabled(false);                // Whether to enable flash or not

        _codeScanner.setDecodeCallback(result -> _viewModel.setScanned(result.getText()));    // _viewModel.setScanned()
        _codeScanner.setErrorCallback(result -> Toast.makeText(this, "Camera initialization error: ${it.message}", Toast.LENGTH_LONG).show());
    }
}