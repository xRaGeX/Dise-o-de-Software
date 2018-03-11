﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PruebaJuegoConsola
{
    public partial class Ventana : Form
    {
        public Ventana()
        {
            InitializeComponent();
        }
        Juego juego = new Juego(8);
        Button[,] buttonArray;

        public void Ventana_Load(object sender, EventArgs e)
        {
            
            juego.iniciarMatriz();
            int horizotal = 30;
            int vertical = 30;
            int size = juego.getSize();
            buttonArray = new Button[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    buttonArray[i, j] = new Button();
                    buttonArray[i, j].Size = new Size(60, 23);
                    buttonArray[i, j].Location = new Point(horizotal, vertical);
                    buttonArray[i, j].Text = juego.getTablero()[i, j];
                    buttonArray[i, j].Tag = i + "," + j;
                    buttonArray[i, j].Click += myEventHandler;
                    vertical += 30;
                    this.Controls.Add(buttonArray[i, j]);
                }
                horizotal += 80;
                vertical = 30;
            }
            
        }

        void myEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Se hizo click a un boton!!");
            Button button = sender as Button;
            List<List<int>> movidas = juego.MovidasPosibles("1");
            String[] indexes = button.Tag.ToString().Split(',');
            int j = Int32.Parse(indexes[0]);
            int i = Int32.Parse(indexes[1]);
            foreach (List<int> movida in movidas)
            {
                if (i == movida[0] && j == movida[1])
                {
                    Console.WriteLine("Esa posicion es correcta!!");
                    juego.realizarJugada("1");
                    updateButtons();
                }
            }
            movidas = juego.MovidasPosibles("1");
            
        }

        public void updateButtons()
        {
            for(int i = 0; i < juego.getSize(); i++)
            {
                for(int j=0;j<juego.getSize(); j++)
                {
                    buttonArray[i, j].Text = juego.getTablero()[i, j];
                }
            }
        }

        /*
        void OnClick(object sender, RoutedEventArgs args)
        {
            
            Button button = sender as Button;
            if (button == )
            {
        
            }
            if (button == button2)
            {
        
            }
        }
        */
    }
}
