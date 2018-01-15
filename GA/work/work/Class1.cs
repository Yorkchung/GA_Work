using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Metaheuristic;

namespace work
{
    class Class1 : GA      //Rosenbrock Problem Solver Class
    {
        int functionNumber;  //函數編號
        int numDimensions;  //維度數

        int ShekelM = 5;
        int startfunction = 13; //start 

        int numRepetitiveRuns = 5;  //重複執行次數
        int numFunctions = 1;  //函數的數量=1; >1 需另存每一函數之Gbest

        double dimensionsLowerBound;  //變數的下限
        double dimensionsUpperBound;  //變數的上限

        double processTime;  //使用時間

        double[] tempFunctionAllGlobalBestFitness1;

        double[,] ShekelAijMatrix = new double[2, 7] { { 4, 1, 8, 6, 3, 2, 5}, { 7, 9, 3, 1, 2, 3,6 } };
        double[] ShekelCiMatrix = new double[7] { 0.1, 0.2, 0.2, 0.4, 0.4, 0.6, 0.3 };

        double[] bestSolutionAverages = new double[23];  //所有Best Solution的平均
        double[] bestSolutionSDs = new double[23];  //所有Best Solution的標準差

        double[] functionAverageProcessTime = new double[23];  //所有的平均使用時間

        double[] functionBestSolutions = new double[23];  //所有Best Solution中最佳的

        string functionName;  //函數名稱

        string[] functionNames = new string[23];  //所有函數名稱
        static void Main(string[] args)
        {
            Class1 ga = new Class1();  //建立Program的實體
            ga.tempFunctionAllGlobalBestFitness1 = new double[ga.numRepetitiveRuns];

            //將計算後的結果寫入檔案
            StreamWriter fileWriter = new StreamWriter("GA-23Function_Gbest.csv");

            fileWriter.WriteLine("Function(Dim), Best, Average, Std, Time(s)");

            Stopwatch stopWatch = new Stopwatch();  //建立計時器

            Console.WriteLine("Function(Dim), Best, Average, Std, Time(s)");

            //ga.startfunction 決定要開啟哪個函式
            for (int function = ga.startfunction; function < ga.numFunctions + ga.startfunction; function++)
            {  //對於23個function
                stopWatch.Reset();  //將計算的時間歸零
                stopWatch.Start();  //開始計算時間

                ga.InitFunction(function + 1);  //呼叫初始函數設定變數內容，問題初始化  因為每個函式的維度、上下限皆不同

                ga.functionNames[function] = ga.functionName;  //紀錄函數名稱

                ga.bestSolutionAverages[function] = 0;

                ga.processTime = 0;

                double[] tempFunctionAllGlobalBestFitness = new double[ga.numRepetitiveRuns];   //初始化所有gbest的暫存地點

                ga.functionBestSolutions[function] = Double.MaxValue;   //因為是最小化問題，所以將gbest先設定為最大化

                for (int run = 0; run < ga.numRepetitiveRuns; run++)
                {  //每一個function各執行ga.numRepetitiveRuns次
                    Console.WriteLine(ga.functionName + "第" + (run + 1) + "次執行");

                    //開始執行GA計算該function的GBestFitness
                    ga.Init(50, ga.numDimensions, ga.dimensionsLowerBound, ga.dimensionsUpperBound, GAOption.EncodingType.Real, GAOption.RepeatableOption.Repeatable); //演算法初始化

                    ga.SetStrategy(GAOption.Select.Roulette_Wheel, GAOption.Crossover.RealNumber.Arithmetic_X, GAOption.Mutation.RealNumber.Gaussian_Mutation); //選擇、交配、突變的方法設定在此

                    ga.Run(160000, 0.9, 0.2);
                    //執行結束

                    tempFunctionAllGlobalBestFitness[run] = ga.GBestFitness;  //紀錄每一次的GBestFitness
                    ga.tempFunctionAllGlobalBestFitness1[run] = ga.GBestFitness;

                    //fileWriter.WriteLine(ga.tempFunctionAllGlobalBestFitness1[run]);

                    ga.bestSolutionAverages[function] += ga.GBestFitness;  //將每一次的GBestFitness紀錄下來，後面可用來計算10次的平均

                    if (ga.GBestFitness < ga.functionBestSolutions[function])
                    {  //紀錄最小(最佳)的GBestFitness
                        ga.functionBestSolutions[function] = ga.GBestFitness;
                    }
                }

                stopWatch.Stop();  //停止計算時間

                ga.processTime = (double)(stopWatch.Elapsed.TotalMilliseconds / 1000);  //紀錄每一個function執行10次的總時間

                Console.WriteLine("process time: " + ga.processTime);

                ga.bestSolutionAverages[function] /= ga.numRepetitiveRuns;  //計算平均的GBestFitness

                ga.bestSolutionSDs[function] = sd(tempFunctionAllGlobalBestFitness);  //計算10次GBestFitness的標準差

                ga.functionAverageProcessTime[function] = ga.processTime / ga.numRepetitiveRuns;  //計算執行10次的平均時間

            }

            //寫出內容
            for (int function = ga.startfunction; function < ga.numFunctions + ga.startfunction; function++)
            {
                Console.WriteLine(ga.functionNames[function] + ", " + ga.functionBestSolutions[function] + ", " + ga.bestSolutionAverages[function] + ", " + ga.bestSolutionSDs[function] + ", " + ga.functionAverageProcessTime[function]);
                fileWriter.WriteLine(ga.functionNames[function] + "," + ga.functionBestSolutions[function] + "," + ga.bestSolutionAverages[function] + "," + ga.bestSolutionSDs[function] + "," + ga.functionAverageProcessTime[function]);

                Console.WriteLine();
                //*******************寫出最佳解GBest內容*****************************


                Console.WriteLine("Best solution found: " + ga.GBest.Length);
                for (int i = 0; i < ga.GBest.Length; i++)
                {
                    Console.Write("{0}, ", ga.GBest[i]);
                }

                fileWriter.WriteLine("Best solution found: ");
                for (int i = 0; i < ga.GBest.Length; i++)
                {
                    fileWriter.Write("{0},", ga.GBest[i]);
                }

                //**************************************************
            }

            fileWriter.Close();
            //寫入檔案結束
            Console.Read();

        }
        public static double sd(double[] fit)
        {
            double sum = 0.0;
            double average;

            for (int i = 0; i < fit.Length; i++)
            {
                sum += fit[i];
            }

            average = sum / fit.Length;

            sum = 0.0;

            for (int i = 0; i < fit.Length; i++)
            {
                sum += (Math.Pow(fit[i] - average, 2));
            }

            return Math.Pow(sum / fit.Length, 0.5);
        }
        public override double Fitness(double[] pos)
        {
            double fitness = 0;

            if (functionNumber == 1)
            {  //Easom
                double ePow = -Math.Pow(pos[0] - Math.PI, 2) - Math.Pow(pos[1] - Math.PI, 2);
                double sum = 0;
                fitness = -Math.Cos(pos[0]) * Math.Cos(pos[1]) * Math.Pow(Math.E, ePow);
            }
            else if (functionNumber == 2)
            {  //Shubert
                double fitness1 = 0, fitness2 = 0;

                for (int j = 1; j <= 5; j++)
                {
                    fitness1 = fitness1 + j * Math.Cos((j + 1) * pos[0] + j);
                    fitness2 = fitness2 + j * Math.Cos((j + 1) * pos[1] + j);
                }

                fitness = fitness1 * fitness2;
            }
            else if (functionNumber == 3 | functionNumber == 10 | functionNumber == 15 | functionNumber == 20)
            {  //Rosenbrock
                for (int j = 0; j < numDimensions - 1; j++)
                {
                    fitness = fitness + 100 * Math.Pow(pos[j + 1] - Math.Pow(pos[j], 2), 2) + Math.Pow(pos[j] - 1, 2);
                }
            }
            else if (functionNumber == 4 | functionNumber == 13 | functionNumber == 18 | functionNumber == 23)
            {  //Zakharov
                double fitness1 = 0, fitness2 = 0;

                for (int j = 0; j < numDimensions; j++)
                {
                    fitness1 = fitness1 + Math.Pow(pos[j], 2);
                    fitness2 = fitness2 + 0.5 * (j + 1) * pos[j];
                }

                fitness = fitness1 + Math.Pow(fitness2, 2) + Math.Pow(fitness2, 4);
            }
            else if (functionNumber == 5 | functionNumber == 9 | functionNumber == 14 | functionNumber == 19)
            {  //Sphere
                for (int j = 0; j < numDimensions; j++)
                {
                    fitness = fitness + Math.Pow(pos[j], 2);
                }
            }
            else if (functionNumber == 6 | functionNumber == 7 | functionNumber == 8)
            {  //Shekel
                double sum;

                for (int n = 0; n < ShekelM; n++)
                {
                    sum = 0;

                    for (int j = 0; j < numDimensions; j++)
                    {
                        sum = sum + Math.Pow(pos[j] - ShekelAijMatrix[j, n], 2);
                    }

                    fitness = fitness + 1 / (sum + ShekelCiMatrix[n]);
                }

                fitness = -fitness;
            }
            else if (functionNumber == 11 | functionNumber == 16 | functionNumber == 21)
            {  //Rastrigin
                for (int j = 0; j < numDimensions; j++)
                {
                    //fitness = fitness + Math.Pow(pos[j], 2) - (10 * Math.Cos(2 * Math.PI * pos[j])) + 10;
                    fitness = fitness + pos[j];
                }
                //fitness = -1.0*fitness;
            }
            else if (functionNumber == 12 | functionNumber == 17 | functionNumber == 22)
            {  //Griewank
                double fitness1 = 0;
                double fitness2 = 1;

                for (int j = 0; j < numDimensions; j++)
                {
                    fitness1 = fitness1 + Math.Pow(pos[j], 2);
                    fitness2 = fitness2 * Math.Cos(pos[j] / Math.Sqrt(j + 1));
                }

                fitness = fitness1 / 4000 - fitness2 + 1;
            }

            return fitness;
        }
        public void InitFunction(int number)
        {
            switch (number)
            {
                //2 numDimensions------------------------------
                case (1):
                    //Easom(2)
                    this.functionName = "Easom(2)";

                    this.functionNumber = 1;

                    this.numDimensions = 2;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = -10;

                    break;

                case (2):
                    //Shubert(2)
                    this.functionName = "Shubert(2)";

                    this.functionNumber = 2;

                    this.numDimensions = 2;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = -10;

                    break;

                case (3):
                    //Rosenbrock(2)
                    this.functionName = "Rosenbrock(2)";

                    this.functionNumber = 3;

                    this.numDimensions = 2;

                    this.dimensionsUpperBound = 30;
                    this.dimensionsLowerBound = -30;

                    break;

                case (4):
                    //Zakharov(2)
                    this.functionName = "Zakharov(2)";

                    this.functionNumber = 4;

                    this.numDimensions = 2;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = -5;

                    break;

                //3 numDimensions------------------------------
                case (5):
                    //De Joung(3)
                    this.functionName = "De Joung(3)";

                    this.functionNumber = 5;

                    this.numDimensions = 3;

                    this.dimensionsUpperBound = 5.12;
                    this.dimensionsLowerBound = -5.12;

                    break;

                //4 numDimensions------------------------------
                case (6):
                    //Shekel(4,5)
                    this.functionName = "Shekel(4.5)";

                    this.functionNumber = 6;

                    this.numDimensions = 4;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = 0;

                    this.ShekelM = 5;

                    break;

                case (7):
                    //Shekel(4,7)
                    this.functionName = "Shekel(4.7)";

                    this.functionNumber = 7;

                    this.numDimensions = 4;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = 0;

                    this.ShekelM = 7;

                    break;

                case (8):
                    //Shekel(4,10)
                    this.functionName = "Shekel(4.10)";

                    this.functionNumber = 8;

                    this.numDimensions = 4;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = 0;

                    this.ShekelM = 10;

                    break;

                //10 numDimensions------------------------------
                case (9):
                    //Sphere(10)
                    this.functionName = "Sphere(10)";

                    this.functionNumber = 9;

                    this.numDimensions = 10;

                    this.dimensionsUpperBound = 100;
                    this.dimensionsLowerBound = -100;

                    break;

                case (10):
                    //Rosenbrock(10)
                    this.functionName = "Rosenbrock(10)";

                    this.functionNumber = 10;

                    this.numDimensions = 10;

                    this.dimensionsUpperBound = 30;
                    this.dimensionsLowerBound = -30;

                    break;

                case (11):
                    //Rastrigin(10)
                    this.functionName = "Rastrigin(10)";

                    this.functionNumber = 11;

                    this.numDimensions = 10;

                    this.dimensionsUpperBound = 5.12;
                    this.dimensionsLowerBound = -5.12;

                    break;

                case (12):
                    //Griewank(10)
                    this.functionName = "Griewank(10)";

                    this.functionNumber = 12;

                    this.numDimensions = 10;

                    this.dimensionsUpperBound = 600;
                    this.dimensionsLowerBound = -600;

                    break;

                case (13):
                    //Zakharov(10)
                    this.functionName = "Zakharov(10)";

                    this.functionNumber = 13;

                    this.numDimensions = 10;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = -5;

                    break;

                //20 numDimensions------------------------------
                case (14):
                    //Sphere(20)
                    this.functionName = "Sphere(20)";

                    this.functionNumber = 14;

                    this.numDimensions = 20;

                    this.dimensionsUpperBound = 100;
                    this.dimensionsLowerBound = -100;

                    break;

                case (15):
                    //Rosenbrock(20)
                    this.functionName = "Rosenbrock(20)";

                    this.functionNumber = 15;

                    this.numDimensions = 20;

                    this.dimensionsUpperBound = 30;
                    this.dimensionsLowerBound = -30;

                    break;

                case (16):
                    //Rastrigin(20)
                    this.functionName = "Rastrigin(20)";

                    this.functionNumber = 16;

                    this.numDimensions = 20;

                    this.dimensionsUpperBound = 5.12;
                    this.dimensionsLowerBound = -5.12;

                    break;

                case (17):
                    //Griewank(20)
                    this.functionName = "Griewank(20)";

                    this.functionNumber = 17;

                    this.numDimensions = 20;

                    this.dimensionsUpperBound = 600;
                    this.dimensionsLowerBound = -600;

                    break;

                case (18):
                    //Zakharov(20)
                    this.functionName = "Zakharov(20)";

                    this.functionNumber = 18;

                    this.numDimensions = 20;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = -5;

                    break;

                //30 numDimensions------------------------------
                case (19):
                    //Sphere(30)
                    this.functionName = "Sphere(30)";

                    this.functionNumber = 19;

                    this.numDimensions = 30;

                    this.dimensionsUpperBound = 100;
                    this.dimensionsLowerBound = -100;

                    break;

                case (20):
                    //Rosenbrock(30)
                    this.functionName = "Rosenbrock(30)";

                    this.functionNumber = 20;

                    this.numDimensions = 30;

                    this.dimensionsUpperBound = 30;
                    this.dimensionsLowerBound = -30;

                    break;

                case (21):
                    //Rastrigin(30)
                    this.functionName = "Rastrigin(30)";

                    this.functionNumber = 21;

                    this.numDimensions = 30;

                    this.dimensionsUpperBound = 5.12;
                    this.dimensionsLowerBound = -5.12;

                    break;

                case (22):
                    //Griewank(30)
                    this.functionName = "Griewank(30)";

                    this.functionNumber = 22;

                    this.numDimensions = 30;

                    this.dimensionsUpperBound = 600;
                    this.dimensionsLowerBound = -600;

                    break;

                case (23):
                    //Zakharov(30)
                    this.functionName = "Zakharov(30)";

                    this.functionNumber = 23;

                    this.numDimensions = 30;

                    this.dimensionsUpperBound = 10;
                    this.dimensionsLowerBound = -5;

                    break;

                default:
                    break;
            }
        }
    }
}
