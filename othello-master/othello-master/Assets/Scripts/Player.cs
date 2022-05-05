using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Tile[] board = new Tile[Constants.NumTiles];
    public Node parent;
    public List<Node> childList = new List<Node>();
    public int type;//Constants.MIN o Constants.MAX
    public double utility;
    public double alfa;
    public double beta;

    public Node(Tile[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            this.board[i] = new Tile();
            this.board[i].value = tiles[i].value;
        }

    }    

}

public class Player : MonoBehaviour
{
    public int turn;    
    private BoardManager boardManager;

    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }
       
    /*
     * Entrada: Dado un tablero
     * Salida: Posición donde mueve  
     */
    public int SelectTile(Tile[] board)
    {        
             
        //Generamos el nodo raíz del árbol (MAX)
        Node root = new Node(board);
        root.type = Constants.MAX;

        //Generamos primer nivel de nodos hijos
        List<int> selectableTiles = boardManager.FindSelectableTiles(board, turn);

        foreach (int s in selectableTiles)
        {
            //Creo un nuevo nodo hijo con el tablero padre
            Node n = new Node(root.board);
            //Lo añadimos a la lista de nodos hijo
            root.childList.Add(n);
            //Enlazo con su padre
            n.parent = root;
            //En nivel 1, los hijos son MIN
            n.type = Constants.MIN;
            //Aplico un movimiento, generando un nuevo tablero con ese movimiento
            boardManager.Move(n.board, s, turn);
            //si queremos imprimir el nodo generado (tablero hijo)
            //boardManager.PrintBoard(n.board);
        }

        foreach (Node nodoHijo in root.childList)
        {   
            //Generamos segundo nivel de nodos hijos
            //generamos los tableros de los hijos de root
            List<int> selectableTilesHijosNv1 = boardManager.FindSelectableTiles(nodoHijo.board, turn);

            foreach (int s in selectableTilesHijosNv1)
            {
                //Creamos los hijos del nivel 2
                Node n = new Node(nodoHijo.board);
                //Lo añadimos a la lista de nodos hijo
                nodoHijo.childList.Add(n);
                //Enlazo con su padre
                n.parent = nodoHijo;
                //En nivel 2, los hijos son MAX
                n.type = Constants.MAX;
                //Aplico un movimiento, generando un nuevo tablero con ese movimiento
                boardManager.Move(n.board, s, turn);
                //si queremos imprimir el nodo generado (tablero hijo)
                //boardManager.PrintBoard(n.board);
            }


        }
        foreach (Node nodoHijo in root.childList)
        {
            foreach (Node nodoHijo2 in nodoHijo.childList)
            {
                //Generamos tercer nivel de nodos hijos
                //generamos los tableros de los hijos
                List<int> selectableTilesHijosNv2 = boardManager.FindSelectableTiles(nodoHijo2.board, turn);

                foreach (int s in selectableTilesHijosNv2)
                {
                    //Creamos los hijos del nivel 2
                    Node n = new Node(nodoHijo2.board);
                    //Lo añadimos a la lista de nodos hijo
                    nodoHijo2.childList.Add(n);
                    //Enlazo con su padre
                    n.parent = nodoHijo2;
                    //En nivel 3, los hijos son MIN
                    n.type = Constants.MIN;
                    //Aplico un movimiento, generando un nuevo tablero con ese movimiento
                    boardManager.Move(n.board, s, turn);
                    //si queremos imprimir el nodo generado (tablero hijo)
                    //boardManager.PrintBoard(n.board);

                }
            }
        }
        //Selecciono un movimiento aleatorio. Esto habrá que modificarlo para elegir el mejor movimiento según MINIMAX
        //int movimiento = Random.Range(0, selectableTiles.Count);
        int movimiento = 0;

        return selectableTiles[movimiento];

    }
    public int funcionUtility(Node nodo, BoardManager bm, int turn, List<int> selectableTiles)
    {
        int fichasDeTuColor = 0;
        int fichasQueCambia = 0;
        //recuperamos las fichas de cada color
        fichasDeTuColor = bm.CountPieces(nodo.board, turn);
        //por cada tile a la que se puede mover, calculamos cuantas cambia y nos quedamos con el maximo
        foreach(int i in selectableTiles)
        {
            List<int> cas = bm.FindSwappablePieces(nodo.board, i, turn);
            if(cas.Count >= fichasQueCambia)
            {
                fichasQueCambia = cas.Count;
            }
        }
        int utilidad = (fichasDeTuColor+fichasQueCambia)/2;
        return utilidad;
    }

}
