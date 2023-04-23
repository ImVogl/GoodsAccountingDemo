package com.goods_account.ui;

import android.arch.lifecycle.ViewModel;
import android.arch.lifecycle.ViewModelProvider;
import android.support.annotation.NonNull;

import com.goods_account.data.LoginDataSource;
import com.goods_account.data.LoginRepository;
import com.goods_account.ui.login.LoginViewModel;
import com.goods_account.ui.sell.SellViewModel;

/**
 * ViewModel provider factory to instantiate LoginViewModel.
 * Required given LoginViewModel has a non-empty constructor
 */
public class ViewModelFactory implements ViewModelProvider.Factory {

    @NonNull
    @Override
    @SuppressWarnings("unchecked")
    public <T extends ViewModel> T create(@NonNull Class<T> modelClass) {
        if (modelClass.isAssignableFrom(LoginViewModel.class)) {
            return (T) new LoginViewModel(LoginRepository.getInstance(new LoginDataSource()));
        } else if (modelClass.isAssignableFrom(SellViewModel.class)) {
            return (T) new SellViewModel();
        } else {
            throw new IllegalArgumentException("Unknown ViewModel class");
        }
    }
}