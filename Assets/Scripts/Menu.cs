using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject menu1;
    public GameObject menu2;

    public TMP_Text dinero_TotalTxt;
    public TMP_Text recordN1;

    Animator animatorM1;
    Animator animatorM2;
    private bool isMenuOut = false;
    private int nivel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animatorM1 = menu1.GetComponent<Animator>();
        animatorM2 = menu2.GetComponent<Animator>();

        menu1.SetActive(true);
        menu2.SetActive(false);

        dinero_TotalTxt.text = "$" + DatosJuego.DineroTotal;
        recordN1.text = "Mejor: " + DatosJuego.ObtenerRecord("Troposfera");
    }

    // Update is called once per frame
    void Update()
    {
        animatorM1.SetBool("MenuOut", isMenuOut);
        animatorM2.SetBool("MenuOut", isMenuOut);
    }

    public void JugarBtn()
    {
        isMenuOut = true;
        StartCoroutine(GoToMenu2());
    }
    IEnumerator GoToMenu2()
    {
        yield return new WaitForSeconds(1f);
        menu1.SetActive(false);
        isMenuOut = false;
        menu2.SetActive(true);
    }

    public void RegresarBtn()
    {
        isMenuOut = true;
        StartCoroutine(BackToMenu1());
    }
    IEnumerator BackToMenu1()
    {
        yield return new WaitForSeconds(1f);
        menu2.SetActive(false);
        isMenuOut = false;
        menu1.SetActive(true);
    }

    public void GoToLevel1()
    {
        nivel = 1;
        isMenuOut = true;
        StartCoroutine(StartLevel());
    }
    public void GoToLevel2()
    {
        nivel = 2;
        isMenuOut = true;
        StartCoroutine(StartLevel());
    }
    IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nivel);
    }
}
