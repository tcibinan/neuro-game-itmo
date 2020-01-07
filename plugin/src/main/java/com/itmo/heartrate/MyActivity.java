package com.itmo.heartrate;

import android.Manifest;
import android.content.pm.PackageManager;
import android.hardware.Sensor;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.view.SurfaceView;
import android.widget.Toast;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.unity3d.player.UnityPlayerActivity;

import net.kibotu.heartrateometer.HeartRateOmeter;

import java.util.concurrent.atomic.AtomicInteger;

import io.reactivex.functions.Consumer;

public class MyActivity extends UnityPlayerActivity {

    private final static AtomicInteger heartRate = new AtomicInteger();

    private SensorManager sensorManager;
    private Sensor sensor;

    @Override
    protected void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        if (!hasPermission(Manifest.permission.CAMERA)) {
            checkPermissions(123, Manifest.permission.CAMERA);
            return;
        }
        final SurfaceView preview = new SurfaceView(this);
        preview.getHolder().setFixedSize(1, 1);
        new HeartRateOmeter()
                .withAverageAfterSeconds(1)
                .bpmUpdates(preview)
                .subscribe(new Consumer<HeartRateOmeter.Bpm>() {
                    @Override
                    public void accept(HeartRateOmeter.Bpm integer) {
                        switch (integer.getType()) {
                            case ON: MyActivity.setHeartRate(integer.getValue()); break;
                            case OFF: MyActivity.setHeartRate(0);
                        }
                    }
                }, new Consumer<Throwable>() {
                    @Override
                    public void accept(Throwable throwable) {
                        throwable.printStackTrace();
                    }
                });
    }

    private void checkPermissions(int callbackId, String... permissionsId) {
        if (!hasPermission(permissionsId)) {
            ActivityCompat.requestPermissions(this, permissionsId, callbackId);
        }
    }

    private boolean hasPermission(String... permissionsId) {
        boolean hasPermission = true;
        for (String permission : permissionsId) {
            hasPermission = hasPermission
                    && ContextCompat.checkSelfPermission(this, permission) == PackageManager.PERMISSION_GRANTED;
        }
        return hasPermission;
    }

    private void toast(String message) {
        Toast.makeText(getApplicationContext(), message, Toast.LENGTH_SHORT).show();
    }

    private static void setHeartRate(int value) {
        heartRate.set(value);
    }

    public static int getHeartRate() {
        return heartRate.get();
    }
}
