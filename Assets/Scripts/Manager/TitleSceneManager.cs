using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    //�p�l���̃I�u�W�F�N�g
    public Image PanelImage;
    //�Ó]����X�s�[�h
    [SerializeField] private float alphaSpeed;
    private bool isSceneChange;
    private bool isChangeEnd;
    private float alphaValue;
    private Color PanelColor;
    // �_�ł�����Ώ�
    [SerializeField] private Image _target;

    // �_�Ŏ���[s]
    [SerializeField] private float _cycle = 1;

    private float _time;

    public void StartGame()
    {
        //�p�l���C���[�W��L���ɂ���
        PanelImage.enabled = true;
        //�V�[���`�F���W�N��
        isSceneChange = true;
    }
    public void EndGame()
    {
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        isSceneChange = false;
        isChangeEnd = false;
        alphaValue = 0f;
        PanelColor = PanelImage.color;
    }

    // Update is called once per frame
    private void Update()
    {
        //�V�[���`�F���W���N�����Ă���Ƃ�
        //�Ó]�@�\
        if (isSceneChange)
        {
            alphaValue += alphaSpeed * Time.deltaTime;
            PanelImage.color = new Color(PanelColor.r, PanelColor.g, PanelColor.b, alphaValue);
            if (alphaValue >= 1)
            {
                isSceneChange = false;
                //�V�[���؂�ւ��t���O
                isChangeEnd = true;
            }
        }
        //�V�[���؂�ւ�
        if (isChangeEnd)
        {
            SceneManager.LoadScene("Game");
        }
        // �����������o�߂�����
        _time += Time.deltaTime;

        // ����cycle�ŌJ��Ԃ��g�̃A���t�@�l�v�Z
        var alpha = Mathf.Cos(2 * Mathf.PI * _time / _cycle) * 0.5f + 0.5f;

        // ��������time�ɂ�����A���t�@�l�𔽉f
        var color = _target.color;
        color.a = alpha;
        _target.color = color;
    }
}
