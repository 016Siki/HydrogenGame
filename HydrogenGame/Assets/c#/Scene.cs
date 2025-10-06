using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    // タイマー変数の初期化
    private float Timer = 0.0f;
    // シーン変更が要求されたかどうかを示すフラグ
    private bool GameChangeSceneRequested = false;
    private bool TitleChangeSceneRequested = false;
    private bool OverChangeSceneRequested = false;
    private bool changeSceneRequested = false;
    private bool OptionChangeSceneRequested = false;
    private bool ExitOption = false;
    private bool Fade = false;

    public GameObject TITLEBUTTON;
    public GameObject GAMEBUTTON;
    public GameObject OVERBUTTON;
    public GameObject ENDBUTTON;
    public GameObject OPTIOHBUTTON;
    public GameObject RANKINGBUTTON;
    public GameObject APIPanel;



    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    // ゲームシーンに変更するメソッド
    public void ChangeGameScene()
    {
        // シーン変更要求フラグを立てる
        changeSceneRequested = true;
        GameChangeSceneRequested = true;
        Fade = true;
        TITLEBUTTON.SetActive(false);
        ENDBUTTON.SetActive(false);
    }
    public void ChangeOverScene()
    {
        // シーン変更要求フラグを立てる
        changeSceneRequested = true;
        OverChangeSceneRequested = true;
        GAMEBUTTON.SetActive(false);
    }

    public void ChangeTitleScene()
    {
        // シーン変更要求フラグを立てる
        changeSceneRequested = true;
        TitleChangeSceneRequested = true;
        Fade = true;
        OVERBUTTON.SetActive(false);
    }

    public void ChangeOptionScene() 
    {
        changeSceneRequested = true;
        OptionChangeSceneRequested = true;
       
    }

    public void PushGameRankingButton()
    {
        APIPanel.SetActive(true);
    }

    // Startは最初のフレームの前に一度呼び出されます
 

    // Updateは毎フレーム呼び出されます
    void Update()
    {
        // シーン変更が要求された場合の処理
        if (changeSceneRequested)
        {
            if(Fade)
            {
                // タイマーを増加させる
                Timer += Time.deltaTime;
                // タイマーが2秒以上経過した場合
                if (Timer >= 2.0f)
                {
                    if (GameChangeSceneRequested)
                    {
                        // "Game"シーンに変更する
                        SceneManager.LoadScene("GameScene");
                        // シーン変更要求フラグをリセット
                        changeSceneRequested = false;
                        GameChangeSceneRequested = false;
                        // タイマーをリセット
                        Timer = 0.0f;
                        Fade = false;
                    }
                    if (TitleChangeSceneRequested)
                    {
                        
                        SceneManager.LoadScene("Title");
                        // シーン変更要求フラグをリセット
                        changeSceneRequested = false;
                        TitleChangeSceneRequested = false;
                        // タイマーをリセット
                        Timer = 0.0f;
                        Fade = false;
                    }
                }
            }
            else
            {
                if (OverChangeSceneRequested)
                {
                    SceneManager.LoadScene("Over");
                    // シーン変更要求フラグをリセット
                    changeSceneRequested = false;
                    OverChangeSceneRequested = false;
                }
                if (OptionChangeSceneRequested)
                {
                    if (ExitOption)
                    {
                        SceneManager.LoadScene("Option");
                        ExitOption = true;
                        changeSceneRequested = false;
                        OptionChangeSceneRequested = false;
                    }
                    else
                    {
                        SceneManager.LoadScene("Title");
                        ExitOption = false;
                        changeSceneRequested = false;
                        OptionChangeSceneRequested = false;
                    }
                }
            }
        }
    }
}
