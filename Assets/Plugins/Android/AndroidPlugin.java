package com.syz.unitygame;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import com.unity3d.player.UnityPlayer;
import java.io.File;

public class AndroidPlugin{
    public void installApk(String path){
        Intent intent = new Intent(Intent.ACTION_VIEW);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
            intent.setDataAndType(Uri.fromFile(new File(path)), "application/vnd.android.package-archive");
        }
        else {
            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            intent.setDataAndType(Uri.fromFile(new File(path)),"application/vnd.android.package-archive");
        }
        UnityPlayer.currentActivity.startActivity(intent);
    }

}