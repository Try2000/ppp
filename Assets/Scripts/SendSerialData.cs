using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SendSerialData : MonoBehaviour
{
    public TextMeshProUGUI err_text;  // �G���[���b�Z�[�W��\�����邽�߂�UI�e�L�X�g
    private AndroidJavaClass androidJavaClass_ = null;  // Android Java�N���X�̎Q��
    private string errMsg_ = "";  // �G���[���b�Z�[�W�̊i�[
    private object lockObject_ = new object();  // �X���b�h�����p�̃I�u�W�F�N�g

    // Start is called before the first frame update
    void Start()
    {
        err_text.enabled = true;  // �G���[�e�L�X�g���\���ɐݒ�
        using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
        using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
        using (AndroidJavaClass pendingIntentClass = new AndroidJavaClass("android.app.PendingIntent"))
        {
            // FLAG_IMMUTABLE �t���O���w�肷��ꍇ
            int flags = pendingIntentClass.GetStatic<int>("FLAG_IMMUTABLE");
            
            // Create a PendingIntent with the IMMUTABLE flag
            AndroidJavaObject pendingIntent = pendingIntentClass.CallStatic<AndroidJavaObject>(
                "getBroadcast", 
                AndroidJNI.AttachCurrentThread(),  // Context
                0,                               // Request code
                intentObject,                     // Intent
                flags                            // Flags
            );

            // Log the pendingIntent to verify creation
            Debug.Log("PendingIntent created: " + pendingIntent);
        }
        TryConnect();
    }
    public void TryConnect()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = null;
        AndroidJavaObject context = null;
        AndroidJavaObject intent = null;
        try
        {
            activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            context = activity.Call<AndroidJavaObject>("getApplicationContext");
            intent = activity.Call<AndroidJavaObject>("getIntent");
        }
        catch (Exception e)
        {
            err_text.text = "call error :" + e.Message;  // ��O�����������ꍇ�̃G���[���b�Z�[�W��\��
        }

        androidJavaClass_ = new AndroidJavaClass("com.hoho.android.usbserial.wrapper.UsbSerialWrapper");
        if (activity == null || context == null || intent == null) return;
        try
        {
            androidJavaClass_.CallStatic("Initialize", context, activity, intent);  // ������
            bool ret = androidJavaClass_.CallStatic<bool>("OpenDevice", 115200);  // �f�o�C�X���J��
            if (!ret)
            {
                err_text.text = androidJavaClass_.CallStatic<string>("ErrorMsg");  // �G���[���b�Z�[�W���擾���ĕ\��
            }
            else
            {
                err_text.text = "Device opened successfully";  // ����Ƀf�o�C�X���J���ꂽ���Ƃ�\��
            }
        }
        catch (Exception e)
        {
            err_text.text = "call error :" + e.Message;  // ��O�����������ꍇ�̃G���[���b�Z�[�W��\��
        }
    }
    int count = 0;
    private void Update()
    {
        count++;
        if (count < 80) return;
        count = 0;
        TryConnect();
    }

    // �V���A���f�[�^�𑗐M���郁�\�b�h
    public void SendData(string data)
    {
        lock (lockObject_)
        {
            bool isWriteSuccess = false;
            try
            {
                if (androidJavaClass_ != null)
                {
                    isWriteSuccess = androidJavaClass_.CallStatic<bool>("Write", data);  // �f�[�^�𑗐M
                    if(isWriteSuccess) errMsg_ = data;
                }
                else
                {
                    errMsg_ = "androidJavaClass_ is null.";  // Java�N���X������������Ă��Ȃ��ꍇ
                }
            }
            catch (Exception e)
            {
                errMsg_ = "Exception :" + e.Message;  // ��O�����������ꍇ�̃G���[���b�Z�[�W
            }

            err_text.text = errMsg_;  // �G���[���b�Z�[�W��\��
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // �K�v�ɉ����ăG���[���b�Z�[�W���X�V����Ȃǂ̏�����ǉ��ł��܂�
    }
}
