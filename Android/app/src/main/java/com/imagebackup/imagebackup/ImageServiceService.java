package com.imagebackup.imagebackup;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.media.Image;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.os.Environment;
import android.os.IBinder;
import android.support.annotation.Nullable;
import android.support.v4.app.NotificationCompat;
import android.support.v4.app.NotificationManagerCompat;
import android.util.Base64;
import android.util.Log;
import android.widget.Toast;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.List;

public class ImageServiceService extends Service {
    BroadcastReceiver wifiReceiver;
    boolean running;

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        running = false;
        final IntentFilter theFilter = new IntentFilter();
        theFilter.addAction("android.net.wifi.supplicant.CONNECTION_CHANGE");
        theFilter.addAction("android.net.wifi.STATE_CHANGE");
        this.wifiReceiver = new BroadcastReceiver() {

            @Override
            public void onReceive(Context context, Intent intent) {
                WifiManager wifiManager = (WifiManager) context.getApplicationContext()
                        .getSystemService(Context.WIFI_SERVICE);

                NetworkInfo networkInfo = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
                if (networkInfo != null) {
                    if (networkInfo.getType() == ConnectivityManager.TYPE_WIFI) {
                        if (networkInfo.getState() == NetworkInfo.State.CONNECTED) {
                            try {
                                new Thread(new Runnable() {
                                    @Override
                                    public void run() {
                                        startTransfer();
                                    }
                                }).start();
                            } catch (Exception e) {
                                Log.e("TRANSFER", "Transfer failed with unknown error");
                            }
                        }
                    }
                }
            }
        };

        this.registerReceiver(this.wifiReceiver, theFilter);

    }

    @Override
    public int onStartCommand(Intent intent, int flag, int startId) {
        Toast.makeText(this, "Image Backup starting...", Toast.LENGTH_SHORT).show();
        return START_STICKY;
    }

    @Override
    public void onDestroy() {
        Toast.makeText(this, "Service ending...", Toast.LENGTH_SHORT).show();
    }

    public void startTransfer() {
        if (running) {
            return;
        }
        running = true;
        try {
            InetAddress serverAddr = InetAddress.getByName("10.0.2.2");

            Socket socket = new Socket(serverAddr, 8001);
            DataOutputStream output = new DataOutputStream(socket.getOutputStream());

            ArrayList<File> imageList = new ArrayList<>();
            try {
                File dcim = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM);
                getImagesFromCamera(imageList, dcim);
            } catch (Exception e) {

                Toast.makeText(this, "Can't access pictures", Toast.LENGTH_SHORT).show();
                Log.e("IO", "DCIM", e);
                return;
            }
            int numOfPictures = imageList.size();
            if (numOfPictures == 0) {
                return;
            }
            String channelId = "ImageServiceApp_Channel";
            CharSequence name = "ImageServiceApp";
            int importance = NotificationManager.IMPORTANCE_LOW;
            NotificationChannel notificationChannel = new NotificationChannel(channelId, name, importance);
            notificationChannel.enableLights(true);
            notificationChannel.setLightColor(Color.GREEN);
            notificationChannel.enableVibration(true);
            notificationChannel.setVibrationPattern(new long[]{100, 200, 300, 400, 500, 400, 300, 200 ,100});
            NotificationManager notificationManager = getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(notificationChannel);

            NotificationManagerCompat notificationManagerCompat = NotificationManagerCompat.from(this);
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channelId);
            builder.setContentTitle("Picture Transfer")
                    .setContentText("Transfer In progress")
                    .setSmallIcon(R.drawable.notification_icon)
                    .setPriority(NotificationCompat.PRIORITY_LOW);
            builder.setProgress(numOfPictures, 0, false);
            notificationManager.notify(1, builder.build());


            int count = 1;
            for (File f : imageList) {
                // start transfer json
                JSONObject startTransferMessage = new JSONObject();
                startTransferMessage.put("CommandID", 7);
                String[] stringArr = new String[]{f.getName()};
                startTransferMessage.put("Args", new JSONArray(stringArr));
                startTransferMessage.put("RequestDirPath", "");
                String startTransferString = startTransferMessage.toString();

                // finish transfer json
                JSONObject finishTransferMessage = new JSONObject();
                finishTransferMessage.put("CommandID", 8);
                String[] stringArgs = new String[]{};
                finishTransferMessage.put("Args", new JSONArray(stringArgs));
                finishTransferMessage.put("RequestDirPath", "");
                String finishTransferString = finishTransferMessage.toString();

                count++;
                builder.setProgress(numOfPictures, count, false);
                notificationManager.notify(1, builder.build());


                byte[] toSendBytes;
                toSendBytes = startTransferString.getBytes("ASCII");
                int toSendLen = toSendBytes.length;
                byte[] toSendLenBytes = new byte[4];
                toSendLenBytes[0] = (byte) (toSendLen & 0xff);
                toSendLenBytes[1] = (byte) ((toSendLen >> 8) & 0xff);
                toSendLenBytes[2] = (byte) ((toSendLen >> 16) & 0xff);
                toSendLenBytes[3] = (byte) ((toSendLen >> 24) & 0xff);
                output.write(toSendLenBytes);
                output.flush();
                output.write(toSendBytes);
                output.flush();

                // encode image to base64 and send it

                toSendBytes = Files.readAllBytes(f.toPath());
                toSendLen = toSendBytes.length;
                toSendLenBytes[0] = (byte) (toSendLen & 0xff);
                toSendLenBytes[1] = (byte) ((toSendLen >> 8) & 0xff);
                toSendLenBytes[2] = (byte) ((toSendLen >> 16) & 0xff);
                toSendLenBytes[3] = (byte) ((toSendLen >> 24) & 0xff);
                output.write(toSendLenBytes);
                output.flush();
                output.write(toSendBytes);
                output.flush();


                // finish transfer message
                toSendBytes = finishTransferString.getBytes("ASCII");
                toSendLen = toSendBytes.length;
                toSendLenBytes[0] = (byte) (toSendLen & 0xff);
                toSendLenBytes[1] = (byte) ((toSendLen >> 8) & 0xff);
                toSendLenBytes[2] = (byte) ((toSendLen >> 16) & 0xff);
                toSendLenBytes[3] = (byte) ((toSendLen >> 24) & 0xff);
                output.write(toSendLenBytes);
                output.flush();
                output.write(toSendBytes);
                output.flush();
            }
            builder.setProgress(0, 0, false)
                    .setContentText("Backup complete");
            notificationManager.notify(1, builder.build());
        } catch (Exception e) {
            Log.e("TRANSFER", e.toString());
        }
    }

    public void getImagesFromCamera(List<File> list, File dir) {
        String[] okFileExtensions = new String[]{"jpg", "png", "png", "bmp"};
        File[] files = dir.listFiles();
        if (files != null) {
            for (File file : files) {
                if (file.isDirectory()) {
                    getImagesFromCamera(list, file);
                } else {
                    // if is image
                    for (String extension : okFileExtensions) {
                        if (file.getName().toLowerCase().endsWith((extension))) {
                            list.add(file);
                        }
                    }
                }
            }
        }
    }

}
