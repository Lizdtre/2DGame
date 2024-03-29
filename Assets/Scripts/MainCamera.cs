using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCamera : MonoBehaviour
{   
    private GameObject target;      // O objeto que a c�mera segue (alvo)
    public Collider2D area;         // A regi�o em que a c�mera est�
    public float SpeedFactor = 3f;// A Velocidade de aproxima��o da c�mera
    public bool smoothMovement = false;

    private Camera m_Camera;        // Acesso ao script Camera
    private float aspectRatioOffset;// Compensa��o ao tamanho horizontal da camera ao depender do formato da tela
    private const float zPos = -10; // A posi��o padr�o da c�mera no eixo Z

    public static MainCamera Instance  // Propriedade est�tica para facilitar o acesso da c�mera por outros scripts (singleton)
    {
        get
        {
            return FindObjectOfType<MainCamera>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();

        // Usamos o tamanho vertical da camera e as dimens�es da tela para calcular o tamanho horizontal
        aspectRatioOffset = (((float)Screen.width) / ((float)Screen.height)) * m_Camera.orthographicSize; 
        
        // Iniciaremos a posi��o da c�mera � mesma posi��o do jogador
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, zPos);
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 newPos;

        newPos = target.transform.position; // Posi��o desejada (posi��o do jogador/alvo)

        if(area != null)
        {
            /*
              Essa parte � o que impede a camera de sair das �reas de cada sala
              Cada uma dessas condi��es garante que a camera n�o passe das bordas esquerda, direita, superior e inferior
              
              Para isso verificamos se a borda da c�mera (newPos.x|y +/- [Tamanho da Camera]) n�o est� al�m da borda oposta da tela (area.bounds.center.x|y +/- area.bounds.extents.x|y)
              
              Para as bordas verticais, podemos verificar o tamanho da c�mera usando a propriedade ortographicSize
              Por�m, para as horizontais, devemos levar em considera��o o formato da tela do usu�rio. Fizemos esse c�lculo em start()
             */

            if (newPos.x - aspectRatioOffset < area.bounds.center.x - area.bounds.extents.x)                // ESQUERDA
            {
                newPos.x = (area.bounds.center.x - area.bounds.extents.x) + aspectRatioOffset;
            }
            else if (newPos.x + aspectRatioOffset > area.bounds.center.x + area.bounds.extents.x)           // DIREITA
            {
                newPos.x = (area.bounds.center.x + area.bounds.extents.x) - aspectRatioOffset;
            }

            if (newPos.y - m_Camera.orthographicSize < area.bounds.center.y - area.bounds.extents.y)        // SUPERIOR
            {
                newPos.y = (area.bounds.center.y - area.bounds.extents.y) + m_Camera.orthographicSize;
            }
            else if (newPos.y + m_Camera.orthographicSize > area.bounds.center.y + area.bounds.extents.y)   // INFERIOR
            {
                newPos.y = (area.bounds.center.y + area.bounds.extents.y) - m_Camera.orthographicSize;
            }
        }

        Vector2 distance =  newPos - (Vector2)transform.position; // Distancia entre a posi��o final e a posi��o atual

        if(!smoothMovement)
        {
            transform.position = newPos; // Caso a c�mera ja esteja perto da personagem, pulamos diretamente � ela
        }
        else
        {
            transform.position += (Vector3)distance * ((float)-1.957 * (Mathf.Pow(0.6f, Time.deltaTime)-1)) * SpeedFactor;
            /* Caso contr�rio, aproxima a c�mera da personagem de forma exponencial
             Movimento desejado �  transform.position += distance * 0.6
             Por�m ao levar em considera��o deltaTime, a f�rmula que corretamente aproxima esse movimento �  distance * -1.957 * (0.6^deltaTime - 1)
             F�rmula obtida atrav�s da integral definida de 0.6^x entre 0 e deltaTime
            */
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, zPos); // Mantemos a posi��o Z da C�mera no valor padr�o
    }

    public void FindTarget()    // Atribui��o de alvo da c�mera (ser� o player, por padr�o)
    {
        target = GameObject.Find("Player");
    }

    public void FindTarget(GameObject target) // Atribui��o de alvo da c�mera (quando especificado algo diferente do jogador)
    {
        this.target = target;
    }

    public void ChangeArea(GameObject newArea)  // Mudan�a de �rea da c�mera
    {
        area = newArea.GetComponent<Collider2D>();

        m_Camera.orthographicSize = area.GetComponent<ScreenZone>().cameraSize; // Utilizamos o tamanho de c�mera especificado pela �rea

        // Usamos o tamanho vertical da camera e as dimens�es da tela para calcular o tamanho horizontal
        aspectRatioOffset = (((float)Screen.width) / ((float)Screen.height)) * m_Camera.orthographicSize;
    }
}
