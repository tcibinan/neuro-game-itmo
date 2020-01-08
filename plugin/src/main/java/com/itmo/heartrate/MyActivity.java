package com.itmo.heartrate;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import android.view.SurfaceView;
import android.widget.Toast;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.unity3d.player.UnityPlayerActivity;

import net.kibotu.heartrateometer.HeartRateOmeter;

import java.util.Date;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.atomic.AtomicReference;

import io.reactivex.functions.Consumer;

public class MyActivity extends UnityPlayerActivity {

    private static final long SECOND = 1_000;
    private static final AtomicInteger heartRate = new AtomicInteger();
    private static final AtomicInteger latestNonNullHeartRate = new AtomicInteger();
    private static final AtomicReference<Date> latestNonNullHeartRateDate = new AtomicReference<>();
    private static final AtomicReference<Date> heartRateDate = new AtomicReference<>();

    @Override
    protected void onCreate(final Bundle bundle) {
        super.onCreate(bundle);
        if (!hasPermission(Manifest.permission.CAMERA)) {
            checkPermissions(123, Manifest.permission.CAMERA);
            return;
        }
        final SurfaceView preview = new SurfaceView(this);
        preview.getHolder().setFixedSize(1, 1);
        new HeartRateOmeter()
                .withAverageAfterSeconds(1)
                .observeBpmUpdates(preview)
                .subscribe(new Consumer<Integer>() {
                    @Override
                    public void accept(Integer integer) {
                        Log.d("HeartRateOmeter", "neuro-rate: " + integer.toString());
                        MyActivity.setHeartRate(integer);
                    }
                }, new Consumer<Throwable>() {
                    @Override
                    public void accept(Throwable throwable) {
                        throwable.printStackTrace();
                    }
                });
    }

    private void checkPermissions(final int callbackId, final String... permissionsId) {
        if (!hasPermission(permissionsId)) {
            ActivityCompat.requestPermissions(this, permissionsId, callbackId);
        }
    }

    private boolean hasPermission(final String... permissionsId) {
        boolean hasPermission = true;
        for (String permission : permissionsId) {
            hasPermission = hasPermission
                    && ContextCompat.checkSelfPermission(this, permission) == PackageManager.PERMISSION_GRANTED;
        }
        return hasPermission;
    }

    private static void setHeartRate(final int value) {
        final Date now = new Date();
        heartRate.set(value);
        heartRateDate.set(now);
        if (value > 0) {
            latestNonNullHeartRate.set(value);
            latestNonNullHeartRateDate.set(now);
        }
    }

    public static int getHeartRate() {
        int currentHeartRate = heartRate.get();
        int currentNonNullHeartRate = latestNonNullHeartRate.get();
        final Date currentHeartRateDate = heartRateDate.get();
        final Date currentNonNullHeartRateDate = latestNonNullHeartRateDate.get();
        if (currentHeartRateDate != null && !wasRecently(currentHeartRateDate)) {
            return 0;
        }
        if (currentHeartRate != 0) {
            return currentHeartRate;
        } else if (currentNonNullHeartRateDate != null && wasRecently(currentNonNullHeartRateDate)) {
            return currentNonNullHeartRate;
        } else {
            return 0;
        }
    }

    private static boolean wasRecently(final Date date) {
        return date.getTime() > new Date().getTime() - 3 * SECOND;
    }
}
