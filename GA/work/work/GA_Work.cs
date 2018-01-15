using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Metaheuristic;

namespace work
{
    class GA_Work : GA
    {
        public struct realdistribution {
            public int employee;
        }
        static int[,] need = new int[2, 7] { { 3, 2, 5, 8 ,5,6,8}, { 9, 9, 1, 3,3,4,6 } };//需求表
        static int[,,] dislike = new int[60,2, 7] ;//喜好表
        static int[] man = new int[60];//染色體//存討厭值
        static realdistribution[,] real = new realdistribution[2, 7];//存單時段的員工
        static void Main(string[] args)
        {
            StreamWriter fileWriter = new StreamWriter("work.csv");
            fileWriter.WriteLine("Function(Dim), Best, Average, Std, Time(s)");
            
            GA_Work ga = new GA_Work();
            Stopwatch stop = new Stopwatch();
            
            
            Random rn = new Random();
            for(int i=0;i<man.Length;i++) {
                for(int j=0;j<2;j++) {
                    for (int k = 0; k < 7; k++)
                        dislike[i, j, k] = (rn.Next(1,5));
                }
            }
            Console.WriteLine("人力需求表");
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 7; j++)
                    Console.Write(need[i, j]+"--");
                Console.WriteLine();
            }
            Console.WriteLine("第一條染色體");
            for (int i = 0; i < man.Length; i++) {
                int a = rn.Next(0, 2);
                int b = rn.Next(0, 7);
                man[i] = dislike[i, a, b];//亂數塞時段給員工
                if (i % 5 == 0)
                    Console.WriteLine();
                Console.Write("(" + a + "," + b + ")---" + dislike[i, a, b]);//第i員工排的時段
            }
            Console.WriteLine();
            //fitness
            int sum = 0;
            for (int i = 0 ; i < man.Length; i++)
                sum += man[i];
            
            Console.WriteLine("第一條染色體的fitness值: "+sum);
            /*
            for (int i = 0; i < 60; i++)
            {
                if (i % 5 == 0)
                    Console.WriteLine();
                Console.Write(man[i]+"--");
            }*/



            ga.Init(100,60,1,14, GAOption.EncodingType.Real, GAOption.RepeatableOption.Repeatable);//初始化
            ga.SetStrategy(GAOption.Select.Roulette_Wheel, GAOption.Crossover.RealNumber.Arithmetic_X, GAOption.Mutation.RealNumber.Gaussian_Mutation);//天擇、交配、突變的方法
            //ga.Update();//
            ga.Run(10000,0.9,0.25);//(次數，交配機率，突變機率)
            for (int i = 0; i < ga.GBest.Length; i++)
            {
                fileWriter.Write("{0},", ga.GBest[i]);
            }
            fileWriter.WriteLine();
            fileWriter.Write("{0}",ga.GBestFitness);
            Console.WriteLine("GbestFitness的值: {0}", ga.GBestFitness);
            Console.WriteLine("Gbest");
            int[] newGbest = new int[ga.GBest.Length];
            for (int i = 0; i < ga.GBest.Length; i++)
                newGbest[i] = (int)ga.GBest[i];//double轉int
            int[] fitness = ga.time(newGbest);
            for(int j=0;j<newGbest.Length;j++) {
                Console.Write("位置:"+newGbest[j] + "  討厭程度:" + fitness[j] + "--");
                if ((j + 1) % 5 == 0)
                    Console.WriteLine();
            }
            Console.WriteLine("各時間的員工數");
            for (int i = 0; i < 2; i++){
                for (int j = 0; j < 7; j++)
                    Console.Write(real[i, j].employee + "--");
                Console.WriteLine();
            }
            fileWriter.Close();
            Console.Read();
        }
        public override double Fitness(double[] pos)
        {
            int[] pos2 = new int[pos.Length];
            for (int i = 0; i < pos.Length; i++) 
                pos2 [i] = Convert.ToInt32(pos[i]);
            int[] fitness = time(pos2);
            int sum = 0;
            for (int i = 0; i < pos2.Length; i++)
                sum += fitness[i];
            /*
            for (int i = 0; i < pos.Length; i++)//算fitness，但用1-14加
                fitness += pos2[i];
                *//*
            for(int i=0;i<pos.Length;i++)//印出單一染色體內所有的值
                Console.WriteLine("pos {0} {1}",i,pos[i]);*/
            return sum;
        }
        public int[] time(int[] pos2) {
            int[] fitness = new int[pos2.Length];
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 7; j++)
                    real[i, j].employee =0 ;
            for (int k = 0; k < pos2.Length; k++)
            {
                switch (pos2[k])
                {
                    case (1):
                        fitness[k] = dislike[k, 0, 0];
                        real[0, 0].employee++;
                        break;
                    case (2):
                        fitness[k] = dislike[k, 0, 1];
                        real[0, 1].employee++;
                        break;
                    case (3):
                        fitness[k] = dislike[k, 0, 2];
                        real[0, 2].employee++;
                        break;
                    case (4):
                        fitness[k] = dislike[k, 0, 3];
                        real[0, 3].employee++;
                        break;
                    case (5):
                        fitness[k] = dislike[k, 0, 4];
                        real[0, 4].employee++;
                        break;
                    case (6):
                        fitness[k] = dislike[k, 0, 5];
                        real[0, 5].employee++;
                        break;
                    case (7):
                        fitness[k] = dislike[k, 0, 6];
                        real[0, 6].employee++;
                        break;
                    case (8):
                        fitness[k] = dislike[k, 1, 0];
                        real[1, 0].employee++;
                        break;
                    case (9):
                        fitness[k] = dislike[k, 1, 1];
                        real[1, 1].employee++;
                        break;
                    case (10):
                        fitness[k] = dislike[k, 1, 2];
                        real[1, 2].employee++;
                        break;
                    case (11):
                        fitness[k] = dislike[k, 1, 3];
                        real[1, 3].employee++;
                        break;
                    case (12):
                        fitness[k] = dislike[k, 1, 4];
                        real[1, 4].employee++;
                        break;
                    case (13):
                        fitness[k] = dislike[k, 1, 5];
                        real[1, 5].employee++;
                        break;
                    case (14):
                        fitness[k] = dislike[k, 1, 6];
                        real[1, 6].employee++;
                        break;
                    default:
                        break;
                }
            }
            return fitness;
        }
    }
}
