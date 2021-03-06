﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Threading;

namespace PruebaJuegoConsola
{
    class Juego
    {
        int size;//el tamano del tablero
        String[,] tablero;//el tablero de juego
        List<List<int>> jugadasPosibles;//lista de jugadas posibles para el jugador
        String jugador,ganador;
        String rival;
        int fichasJ1, fichasJ2, dificultad;
        bool juegoTerminado;
        public Juego(int size, int level)
        {
            this.size = size;
            this.jugador = "1";
            this.rival = "2";
            this.tablero = new String[this.size, this.size];
            this.fichasJ1 = 0;
            this.fichasJ2 = 0;
            this.dificultad = level;
            iniciarMatriz();
            this.juegoTerminado = false;
            this.ganador = "-1";
        }

        public String[,] getTablero()
        {
            return this.tablero;
        }

        public int getSize()
        {
            return this.size;
        }

        public void setJugador(String j)
        {
            if (j == "1")
            {
                this.jugador = j;
                this.rival = "2";
            }
            else
            {
                this.jugador = j;
                this.rival = "1";
            }
        }

        public String getJugador()
        {
            return this.jugador;
        }

        public String getRival()
        {
            return this.rival;
        }

        public int getFichasJ1()
        {
            return this.fichasJ1;
        }

        public int getFichasJ2()
        {
            return this.fichasJ2;
        }

        public String getGanador()
        {
            return this.ganador;
        }

        public bool getJuegoTerminado()
        {
            return this.juegoTerminado;
        }

        public Data jugarSistemaVSistema(int x, int y)
        {
            turnoSistema();

            Data data = new Data {
                fichas_J1 = this.fichasJ1,
                fichas_J2 = this.fichasJ2,
                tablero = this.tablero,
                movidas = this.MovidasPosibles(),
                jugador_Actual = this.jugador
            };
            return data;
        }

        public Data jugarJugadorVSistema(int x, int y)
        {
            if(this.jugador == "1")
            {
                realizarJugada(x, y, false);
            }
            else
            {
                turnoSistema();
            }
            Data data = new Data
            {
                fichas_J1 = this.fichasJ1,
                fichas_J2 = this.fichasJ2,
                tablero = this.tablero,
                movidas = this.MovidasPosibles(),
                jugador_Actual = this.jugador
            };
            return data;
        }

        public String[,] clonarTablero(String[,] tablero)
        {
            String[,] copia = new string[this.size, this.size];
            for(int i = 0; i < this.size; i++)
            {
                for (int j =0; j < this.size; j++)
                {
                    copia[i, j] = tablero[i, j];
                }
            }
            return copia;
        }

        public void setFichas()
        {
            int j1=0;
            int j2 = 0;
            for(int i = 0; i < this.size; i++)
            {
                for(int j = 0; j < this.size; j++)
                {
                    if (this.tablero[i, j] == "1")
                    {
                        j1++;
                    }
                    else if(this.tablero[i, j] == "2")
                    {
                        j2++;
                    }
                }
            }
            this.fichasJ1 = j1;
            this.fichasJ2 = j2;
        }

        public int calcularMejor(Dictionary<int, int> numeros)
        {
            int mayor = -1;
            if (numeros.Count == 1)
            {
                mayor = numeros.FirstOrDefault().Key;
            }
            else
            {
                for (int i = 0; i < numeros.Count; i++)
                {
                    int num = numeros.ElementAt(i).Value;
                    if (mayor < 0 || num >= mayor) mayor = numeros.ElementAt(i).Key;
                }
            }
            return mayor;
        }

        public int calcularPeor(Dictionary<int,int> numeros)
        {
            int menor = -1;
            if (numeros.Count == 1)
            {
                menor = numeros.FirstOrDefault().Key;
            }
            else
            {
                for (int i = 0; i < numeros.Count; i++)
                {
                    int num = numeros.ElementAt(i).Value;
                    if (menor < 0 || num <= menor) menor = numeros.ElementAt(i).Key;
                }
            }
            return menor;
        }

        public void turnoSistema()
        {//funcion para que el sistema realice una movida
            List<List<int>> movidasPosibles = this.MovidasPosibles();
            Dictionary<int, int> movidasFinales = new Dictionary<int, int>();
            for(int i = 0; i < movidasPosibles.Count; i++)
            {
                movidasFinales.Add(i, cuantasCome(movidasPosibles[i][0], movidasPosibles[i][1]));
            }

            if (this.dificultad == 2)//si elige dificultad media
            {
                Random rnd = new Random();
                int mejor = calcularMejor(movidasFinales);
                int peor = calcularPeor(movidasFinales);
                if (movidasFinales.Count <= 2)
                {
                    int pos = movidasFinales.ElementAt(rnd.Next(movidasFinales.Count)).Key;
                    this.realizarJugada(movidasPosibles[pos][0], movidasPosibles[pos][1], false);
                }
                else
                {
                    int intentos = 0;
                    int num = rnd.Next(movidasFinales.Count);
                    while (num == mejor || num == peor || intentos<=20)
                    {
                        num = rnd.Next(movidasFinales.Count);
                    }
                }
                
            }
            else if(this.dificultad==1)
            {
                int pos = calcularPeor(movidasFinales);
                this.realizarJugada(movidasPosibles[pos][0], movidasPosibles[pos][1], false);
            }
            else if (this.dificultad == 3)
            {
                int pos = calcularMejor(movidasFinales);
                this.realizarJugada(movidasPosibles[pos][0], movidasPosibles[pos][1], false);
            }
            
            //this.setJugador("1");
        }

        //funcion que calcula las fichas del jugador que se puede comer en cada jugada del sistema.
        public int cuantasCome(int fila, int columna)
        {
            int fichasComidas = 0;
            //CLON DEL TABLERO ORIGINAL PARA REALIZAR LA JUGADA Y LUEGO VOLVER A DEJARLO EN SU FORMA ORIGINAL
            String[,] clon = clonarTablero(this.tablero);
            realizarJugada(fila, columna, true);
            this.juegoTerminado = false;
            fichasComidas = this.fichasJ2;
            //se restauran las fichas y el tablero
            this.tablero = clonarTablero(clon);
            setFichas();
            return fichasComidas;//retorna la cantidad de fichas que tendría el sistema si realiza esa jugada
        }

        public void getJugadasPosibles()
        {
            foreach(List<int> jugada in jugadasPosibles)
            {
                Console.Write("[" + jugada[0] + "," + jugada[1] + "] ");
            }
            Console.Write("\n");
        }

        public void checkJuegoTerminado()
        {
            MovidasPosibles();//se revisa que jugadas posibles tiene el jugador antes de su turno
            if (this.jugadasPosibles.Count == 0)
            {//si alguno de los jugadores se queda sin movimientos
                this.jugador = this.rival;//se cambia de jugador para revisar si tiene jugadas posibles
                MovidasPosibles();//se revisa que movidas tiene disponibles
                if (this.jugadasPosibles.Count == 0)
                {//si el rival tampoco movidas posibles
                    this.juegoTerminado = true;//el juego termina
                    if (this.fichasJ1 > this.fichasJ2)
                    {
                        this.ganador = "Ha ganado el jugador 1!";
                    }
                    else if (this.fichasJ2 > this.fichasJ1)
                    {
                        this.ganador = "Ha ganado el jugador 2!";
                    }
                    else
                    {
                        this.ganador = "Empate!";
                    }
                }
                
                
            }
        }

        //funcion que inicializa el tablero, llenandolo de 0s
        public void iniciarMatriz()
        {
            //llena la tabla con espacios vacios
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    this.tablero[i, j] = "0";
                }
            }
            //coloca las fichas en las posiciones iniciales
            decimal centro = this.size / 2;
            int centroRedondeado = (int) Math.Truncate(centro);
            this.tablero[centroRedondeado, centroRedondeado] = "1";
            this.tablero[centroRedondeado - 1, centroRedondeado - 1] = "1";
            this.tablero[centroRedondeado - 1, centroRedondeado] = "2";
            this.tablero[centroRedondeado, centroRedondeado - 1] = "2";

            this.setFichas();
            
        }

        //funcion que retorna una lista con todas las posibles jugadas que tiene el jugador de turno.
        public List<List<int>> MovidasPosibles()
        {
            //lista con las movidas posibles en base a cual es el jugador a turno
            List<List<int>> movidas = new List<List<int>>();
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (this.tablero[i, j] == this.jugador)//si se encuentra una ficha del jugador
                    {//busca las movidas posibles para esa ficha
                        List<List<int>> movidasFicha = evaluarMovidas(i, j);
                        foreach (List<int> lista in movidasFicha)
                        {
                            if(lista.Count>0)
                            movidas.Add(lista);//agrega las movidas de esa ficha a la lista general de movidas
                        }
                    }
                }
            }
            this.jugadasPosibles = movidas;
            return movidas;
        }

        public List<List<int>> evaluarMovidas(int fila, int columna)
        {//evalua segun la ficha del usuario indicada, si tiene movidas en base a esa ficha
            List<List<int>> movidas = new List<List<int>>();
            String arriba, abajo, izq, der, diagArribaIzq, diagArribaDer, diagAbajoIzq, diagAbajoDer;
            if(Enumerable.Range(1,this.size-2).Contains(fila) && Enumerable.Range(1, this.size - 2).Contains(columna))
            {
                arriba = this.tablero[fila - 1, columna];
                abajo = this.tablero[fila + 1, columna];
                izq = this.tablero[fila, columna - 1];
                der = this.tablero[fila, columna + 1];
                diagArribaIzq = this.tablero[fila - 1, columna - 1];
                diagAbajoIzq = this.tablero[fila + 1, columna - 1];
                diagArribaDer = this.tablero[fila - 1, columna + 1];
                diagAbajoDer = this.tablero[fila + 1, columna + 1];

                if (arriba == this.rival)
                {
                    List<int> movidaArriba = evaluarArriba(fila, columna);
                    movidas.Add(movidaArriba);
                }
                if (abajo == this.rival)
                {
                    List<int> movidaAbajo = evaluarAbajo(fila, columna);
                    movidas.Add(movidaAbajo);
                }
                if (izq == this.rival)
                {
                    List<int> movidaIzq = evaluarIzq(fila, columna);
                    movidas.Add(movidaIzq);
                }
                if (der == this.rival)
                {
                    List<int> movidaDer = evaluarDer(fila, columna);
                    movidas.Add(movidaDer);
                }

                if (diagAbajoDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagAbajoDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }
                if (diagAbajoIzq == this.rival)
                {
                    List<int> movidaDiagAbajoIzq = evaluarDiagAbajoIzq(fila, columna);
                    movidas.Add(movidaDiagAbajoIzq);
                }
                if (diagArribaDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagArribaDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }
                if (diagArribaIzq == this.rival)
                {
                    List<int> movidaDiagArribaIzq = evaluarDiagArribaIzq(fila, columna);
                    movidas.Add(movidaDiagArribaIzq);
                }

            }

            else if (fila<=0 && Enumerable.Range(1, this.size - 2).Contains(columna))
            {//si la ficha esta chocando con la pared superior del tablero
                abajo = this.tablero[fila + 1, columna];
                izq = this.tablero[fila, columna - 1];
                der = this.tablero[fila, columna + 1];
                diagAbajoIzq = this.tablero[fila + 1, columna - 1];
                diagAbajoDer = this.tablero[fila + 1, columna + 1];

                if (abajo == this.rival)
                {
                    List<int> movidaAbajo = evaluarAbajo(fila, columna);
                    movidas.Add(movidaAbajo);
                }
                if (izq == this.rival)
                {
                    List<int> movidaIzq = evaluarIzq(fila, columna);
                    movidas.Add(movidaIzq);
                }
                if (der == this.rival)
                {
                    List<int> movidaDer = evaluarDer(fila, columna);
                    movidas.Add(movidaDer);
                }

                if (diagAbajoDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagAbajoDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }
                if (diagAbajoIzq == this.rival)
                {
                    List<int> movidaDiagAbajoIzq = evaluarDiagAbajoIzq(fila, columna);
                    movidas.Add(movidaDiagAbajoIzq);
                }
            }
            
            else if(fila>=this.size - 1 && Enumerable.Range(1, this.size - 2).Contains(columna))
            {//si la ficha esta chocando con la pared posterior
                arriba = this.tablero[fila - 1, columna];
                izq = this.tablero[fila, columna - 1];
                der = this.tablero[fila, columna + 1];
                diagArribaIzq = this.tablero[fila - 1, columna - 1];
                diagArribaDer = this.tablero[fila - 1, columna + 1];

                if (arriba == this.rival)
                {
                    List<int> movidaArriba = evaluarArriba(fila, columna);
                    movidas.Add(movidaArriba);
                }
                if (izq == this.rival)
                {
                    List<int> movidaIzq = evaluarIzq(fila, columna);
                    movidas.Add(movidaIzq);
                }
                if (der == this.rival)
                {
                    List<int> movidaDer = evaluarDer(fila, columna);
                    movidas.Add(movidaDer);
                }
                if (diagArribaDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagArribaDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }
                if (diagArribaIzq == this.rival)
                {
                    List<int> movidaDiagArribaIzq = evaluarDiagArribaIzq(fila, columna);
                    movidas.Add(movidaDiagArribaIzq);
                }
            }
            
            else if(columna<=0 && Enumerable.Range(1, this.size - 2).Contains(fila))
            {//si la ficha esta chocando con la pared izquierda
                arriba = this.tablero[fila - 1, columna];
                abajo = this.tablero[fila + 1, columna];
                der = this.tablero[fila, columna + 1];
                diagArribaDer = this.tablero[fila - 1, columna + 1];
                diagAbajoDer = this.tablero[fila + 1, columna + 1];

                if (arriba == this.rival)
                {
                    List<int> movidaArriba = evaluarArriba(fila, columna);
                    movidas.Add(movidaArriba);
                }
                if (abajo == this.rival)
                {
                    List<int> movidaAbajo = evaluarAbajo(fila, columna);
                    movidas.Add(movidaAbajo);
                }
                if (der == this.rival)
                {
                    List<int> movidaDer = evaluarDer(fila, columna);
                    movidas.Add(movidaDer);
                }

                if (diagAbajoDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagAbajoDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }
                if (diagArribaDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagArribaDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }

            }

            else if (columna >= this.size - 1 && Enumerable.Range(1, this.size - 2).Contains(fila))
            {//si la ficha esta chocando con la pared derecha
                arriba = this.tablero[fila - 1, columna];
                abajo = this.tablero[fila + 1, columna];
                izq = this.tablero[fila, columna - 1];
                diagArribaIzq = this.tablero[fila - 1, columna - 1];
                diagAbajoIzq = this.tablero[fila + 1, columna - 1];

                if (arriba == this.rival)
                {
                    List<int> movidaArriba = evaluarArriba(fila, columna);
                    movidas.Add(movidaArriba);
                }
                if (abajo == this.rival)
                {
                    List<int> movidaAbajo = evaluarAbajo(fila, columna);
                    movidas.Add(movidaAbajo);
                }
                if (izq == this.rival)
                {
                    List<int> movidaIzq = evaluarIzq(fila, columna);
                    movidas.Add(movidaIzq);
                }
                if (diagAbajoIzq == this.rival)
                {
                    List<int> movidaDiagAbajoIzq = evaluarDiagAbajoIzq(fila, columna);
                    movidas.Add(movidaDiagAbajoIzq);
                }
                if (diagArribaIzq == this.rival)
                {
                    List<int> movidaDiagArribaIzq = evaluarDiagArribaIzq(fila, columna);
                    movidas.Add(movidaDiagArribaIzq);
                }

            }

            else if(fila<=0 && columna <= 0)
            {//si la ficha se encuentra en la esquina superior izquierda del tablero
                abajo = this.tablero[fila + 1, columna];
                der = this.tablero[fila, columna + 1];
                diagAbajoDer = this.tablero[fila + 1, columna + 1];

                if (abajo == this.rival)
                {
                    List<int> movidaAbajo = evaluarAbajo(fila, columna);
                    movidas.Add(movidaAbajo);
                }
                if (der == this.rival)
                {
                    List<int> movidaDer = evaluarDer(fila, columna);
                    movidas.Add(movidaDer);
                }

                if (diagAbajoDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagAbajoDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }
            }

            else if(fila>= this.size - 1 && columna <= 0)
            {//si la ficha se encuentra en la esquina inferior izquierda del tablero
                arriba = this.tablero[fila - 1, columna];
                der = this.tablero[fila, columna + 1];
                diagArribaDer = this.tablero[fila - 1, columna + 1];

                if (arriba == this.rival)
                {
                    List<int> movidaArriba = evaluarArriba(fila, columna);
                    movidas.Add(movidaArriba);
                }
                if (der == this.rival)
                {
                    List<int> movidaDer = evaluarDer(fila, columna);
                    movidas.Add(movidaDer);
                }
                if (diagArribaDer == this.rival)
                {
                    List<int> movidaDiagArribaDer = evaluarDiagArribaDer(fila, columna);
                    movidas.Add(movidaDiagArribaDer);
                }
            }

            else if(fila<=0 && columna >= this.size - 1)
            {//si la ficha se encuentra en la esquina superior derecha
                abajo = this.tablero[fila + 1, columna];
                izq = this.tablero[fila, columna - 1];
                diagAbajoIzq = this.tablero[fila + 1, columna - 1];

                if (abajo == this.rival)
                {
                    List<int> movidaAbajo = evaluarAbajo(fila, columna);
                    movidas.Add(movidaAbajo);
                }
                if (izq == this.rival)
                {
                    List<int> movidaIzq = evaluarIzq(fila, columna);
                    movidas.Add(movidaIzq);
                }
                if (diagAbajoIzq == this.rival)
                {
                    List<int> movidaDiagAbajoIzq = evaluarDiagAbajoIzq(fila, columna);
                    movidas.Add(movidaDiagAbajoIzq);
                }
            }

            else
            {//si la ficha se encuentra en la esquina inferior derecha
                arriba = this.tablero[fila - 1, columna];
                izq = this.tablero[fila, columna - 1];
                diagArribaIzq = this.tablero[fila - 1, columna - 1];

                if (arriba == this.rival)
                {
                    List<int> movidaArriba = evaluarArriba(fila, columna);
                    movidas.Add(movidaArriba);
                }
                if (izq == this.rival)
                {
                    List<int> movidaIzq = evaluarIzq(fila, columna);
                    movidas.Add(movidaIzq);
                }
                if (diagArribaIzq == this.rival)
                {
                    List<int> movidaDiagArribaIzq = evaluarDiagArribaIzq(fila, columna);
                    movidas.Add(movidaDiagArribaIzq);
                }
            }

            return movidas;
        }

        public List<int> evaluarDer(int fila, int columna)
        {
            List<int> lista = new List<int>();
            for (int i = columna + 1; i < this.size; i++)//revisa que hay hacia la derecha de la ficha
            {
                if (this.tablero[fila, i] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[fila, i] == "0")//si encontro un espacio vacio
                {
                    lista.Add(fila);
                    lista.Add(i);
                    return lista;
                }
            }

            return lista;
        }

        public List<int> evaluarIzq(int fila, int columna)
        {
            List<int> lista = new List<int>();
            for (int i = columna - 1; i >= 0; i--)//revisa que hay hacia la izquierda de la ficha
            {
                if (this.tablero[fila, i] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[fila, i] == "0")//si encontro un espacio vacio
                {
                    lista.Add(fila);
                    lista.Add(i);
                    return lista;
                }
            }

            return lista;
        }

        public List<int> evaluarAbajo(int fila, int columna)
        {
            List<int> lista = new List<int>();
            for (int i = fila + 1; i < this.size; i++)//revisa que hay hacia abajo de la ficha
            {
                if (this.tablero[i, columna] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[i, columna] == "0")//si encontro un espacio vacio
                {
                    lista.Add(i);
                    lista.Add(columna);
                    return lista;
                }
            }

            return lista;
        }

        public List<int> evaluarArriba(int fila, int columna)
        {
            List<int> lista = new List<int>();
            for (int i = fila - 1; i >= 0; i--)//revisa que hay hacia arriba de la ficha
            {
                if (this.tablero[i, columna] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[i, columna] == "0")//si encontro un espacio vacio
                {
                    lista.Add(i);
                    lista.Add(columna);
                    return lista;
                }
            }

            return lista;
        }

        public List<int> evaluarDiagAbajoDer(int fila, int columna)
        {
            List<int> lista = new List<int>();
            int i = fila + 1; int j = columna + 1;
            while (i < this.size && j < this.size)
            {
                if (this.tablero[i, j] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[i, j] == "0")//si encontro un espacio vacio
                {
                    lista.Add(i);
                    lista.Add(j);
                    return lista;
                }
                i++;
                j++;
            }

            return lista;
        }

        public List<int> evaluarDiagAbajoIzq(int fila, int columna)
        {
            List<int> lista = new List<int>();
            int i = fila + 1; int j = columna - 1;
            while (i < this.size && j >= 0)
            {
                if (this.tablero[i, j] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[i, j] == "0")//si encontro un espacio vacio
                {
                    lista.Add(i);
                    lista.Add(j);
                    return lista;
                }
                i++;
                j--;
            }

            return lista;
        }

        public List<int> evaluarDiagArribaDer(int fila, int columna)
        {
            List<int> lista = new List<int>();
            int i = fila - 1; int j = columna + 1;

            while (i >= 0 && j < this.size)
            {
                if (this.tablero[i, j] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[i, j] == "0")//si encontro un espacio vacio
                {
                    lista.Add(i);
                    lista.Add(j);
                    return lista;
                }
                i--;
                j++;
            }

            return lista;
        }

        public List<int> evaluarDiagArribaIzq(int fila, int columna)
        {
            List<int> lista = new List<int>();
            int i = fila - 1; int j = columna - 1;
            while (i >= 0 && j >= 0)
            {
                if (this.tablero[i, j] == this.jugador)
                {
                    break;
                }
                else if (this.tablero[i, j] == "0")//si encontro un espacio vacio
                {
                    lista.Add(i);
                    lista.Add(j);
                    return lista;
                }
                i--;
                j--;
            }

            return lista;
        }

        //realiza la jugada
        public void realizarJugada(int fila, int columna, bool esPrueba)
        {

            this.tablero[fila, columna] = this.jugador;
            for (int i = 0; i < this.size; i++) 
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (this.tablero[i,j] == this.rival)
                    {
                        List<List<int>> fichasComibles = evaluarFichasComibles(i, j);
                        actualizarTablero(fichasComibles);
                    }
                }
            }

            if (!esPrueba)//Si la movida realizada no es una prueba que no va a tener efecto en el tablero
            {
                if (this.getJugador() == "1")
                {
                    this.setJugador("2");
                }
                else
                {
                    this.setJugador("1");
                }
            }
            

            this.setFichas();//se actualizan las fichas
            checkJuegoTerminado();//se revisa si se termina el juego
        }

        public List<List<int>> evaluarFichasComibles(int fila, int columna)
        {
            List<List<int>> comibles = new List<List<int>>();
            List<List<int>> chkIzq = checkIzq(fila,columna);
            List<List<int>> chkDer = checkDerecha(fila, columna);
            List<List<int>> chkArriba = checkArriba(fila, columna);
            List<List<int>> chkAbajo = checkAbajo(fila, columna);
            List<List<int>> chkArribaIzq = checkArribaIzq(fila, columna);
            List<List<int>> chkArribaDer = checkArribaDer(fila, columna);
            List<List<int>> chkAbajoIzq = checkAbajoIzq(fila, columna);
            List<List<int>> chkAbajoDer = checkAbajoDer(fila, columna);
            if(chkDer != null && chkIzq != null)
            {//si la ficha rival actual se encuentra entre 2 fichas del jugador
                foreach (List<int> lista in chkIzq) comibles.Add(lista);
                foreach (List<int> lista in chkDer) comibles.Add(lista);
            }
            if(chkArriba != null && chkAbajo != null)
            {
                foreach (List<int> lista in chkArriba) comibles.Add(lista);
                foreach (List<int> lista in chkAbajo) comibles.Add(lista);
            }
            if(chkArribaIzq != null && chkAbajoDer != null)
            {
                foreach (List<int> lista in chkArribaIzq) comibles.Add(lista);
                foreach (List<int> lista in chkAbajoDer) comibles.Add(lista);
            }
            if(chkArribaDer != null && chkAbajoIzq != null)
            {
                foreach (List<int> lista in chkArribaDer) comibles.Add(lista);
                foreach (List<int> lista in chkAbajoIzq) comibles.Add(lista);
            }
            return comibles;

        }

        public List<List<int>> checkIzq(int fila, int columna)
        {
            int i = columna;
            List<List<int>> fichas = new List<List<int>>();
            while (i > 0 && this.tablero[fila,i-1]!="0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(fila);
                fichaActual.Add(i);
                fichas.Add(fichaActual);
                if (this.tablero[fila, i - 1] == this.jugador) return fichas;
                i--;
            }
            return null;
            
        }

        public List<List<int>> checkDerecha(int fila, int columna)
        {
            int i = columna;
            List<List<int>> fichas = new List<List<int>>();
            while (i < this.size - 1 && this.tablero[fila, i + 1] != "0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(fila);
                fichaActual.Add(i);
                fichas.Add(fichaActual);
                if (this.tablero[fila, i + 1] == this.jugador) return fichas;
                i++;
            }
            return null;

        }

        public List<List<int>> checkArriba(int fila, int columna)
        {
            int i = fila;
            List<List<int>> fichas = new List<List<int>>();
            while (i > 0 && this.tablero[i - 1, columna] != "0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(i);
                fichaActual.Add(columna);
                fichas.Add(fichaActual);
                if (this.tablero[i - 1, columna] == this.jugador) return fichas;
                i--;
            }
            return null;

        }

        public List<List<int>> checkAbajo(int fila, int columna)
        {
            int i = fila;
            List<List<int>> fichas = new List<List<int>>();
            while (i < this.size - 1 && this.tablero[i + 1, columna] != "0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(i);
                fichaActual.Add(columna);
                fichas.Add(fichaActual);
                if (this.tablero[i + 1, columna] == this.jugador) return fichas;
                i++;
            }
            return null;

        }

        public List<List<int>> checkArribaIzq(int fila, int columna)
        {
            int i = fila; int j = columna;
            List<List<int>> fichas = new List<List<int>>();
            while(i > 0 && j > 0 && this.tablero[i - 1, j - 1] != "0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(i);
                fichaActual.Add(j);
                fichas.Add(fichaActual);
                if (this.tablero[i - 1, j - 1] == this.jugador) return fichas;
                i--;
                j--;
            }

            return null;
        }

        public List<List<int>> checkAbajoIzq(int fila, int columna)
        {
            int i = fila; int j = columna;
            List<List<int>> fichas = new List<List<int>>();
            while (i < this.size - 1 && j > 0 && this.tablero[i + 1, j - 1] != "0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(i);
                fichaActual.Add(j);
                fichas.Add(fichaActual);
                if (this.tablero[i + 1, j - 1] == this.jugador) return fichas;
                i++;
                j--;
            }

            return null;
        }

        public List<List<int>> checkArribaDer(int fila, int columna)
        {
            int i = fila; int j = columna;
            List<List<int>> fichas = new List<List<int>>();
            while (i > 0 && j < this.size - 1 && this.tablero[i - 1, j + 1] != "0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(i);
                fichaActual.Add(j);
                fichas.Add(fichaActual);
                if (this.tablero[i - 1, j + 1] == this.jugador) return fichas;
                i--;
                j++;
            }

            return null;
        }

        public List<List<int>> checkAbajoDer(int fila, int columna)
        {
            int i = fila; int j = columna;
            List<List<int>> fichas = new List<List<int>>();
            while (i < this.size - 1 && j < this.size - 1 && this.tablero[i + 1, j + 1] != "0")
            {
                List<int> fichaActual = new List<int>();
                fichaActual.Add(i);
                fichaActual.Add(j);
                fichas.Add(fichaActual);
                if (this.tablero[i + 1, j + 1] == this.jugador) return fichas;
                i++;
                j++;
            }

            return null;
        }

        public void actualizarTablero(List<List<int>> fichasComibles)
        {
            foreach(List<int> ficha in fichasComibles)
            {
                this.tablero[ficha[0], ficha[1]] = this.jugador;
            }
        }

        //dibuja el tablero en la consola
        public void dibujar()
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    Console.SetCursorPosition(j * 4, i + 1);
                    Console.Write(this.tablero[i, j]);
                }
            }
            Console.Write("\n");
            Console.ReadKey();
        }





    }
}
