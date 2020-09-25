package com.syz.unitygame;
import android.content.Intent;
import android.net.Uri;
import com.unity3d.player.UnityPlayer;
import java.io.File;

public class AndroidPlugin{
    public void installApk(String path){
        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        intent.setDataAndType(Uri.fromFile(new File("link to downloaded file")),"application/vnd.android.package-archive");
        UnityPlayer.currentActivity.startActivity(intent);
    }

}