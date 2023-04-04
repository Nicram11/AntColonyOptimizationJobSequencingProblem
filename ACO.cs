using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSequencingProblemACO
{
    internal class ACO
    {
        //Dostrajanie Algorytmu
        private int antsCount = 100;
        private int iterations = 1000;
        private double evaporationRate = 0.5;
        private double alpha = 1;
        private double beta = 1;

        //Input
        private int jobsCount;
        private int[] deadlines;
        private int[] profits;
        private int[] taskTimes;


        private double[][] pheromones;
        private double[][] heuristicValues;
        private double[][] probabilities;
        private int[][] schedules;


        public int BestProfit { get; private set; }
        public int[] BestSchedule { get; private set; }

        public ACO(int[] deadlines, int[] profits, int[] taskTimes)
        {

            if (deadlines.Length != profits.Length || deadlines.Length != taskTimes.Length) 
                throw new Exception("Invalid Input");
            
            jobsCount = deadlines.Length;

            this.deadlines = deadlines;
            this.profits = profits;
            this.taskTimes = taskTimes;

            Initialize();
        }

        public ACO(int[] deadlines, int[] profits, int[] taskTimes, int antsCount, int maxIterations, double evaporationRate, double alpha, double beta)
        {

            if (deadlines.Length != profits.Length || deadlines.Length != taskTimes.Length)
                throw new Exception("Invalid Input");

            jobsCount = deadlines.Length;

            this.deadlines = deadlines;
            this.profits = profits;
            this.taskTimes = taskTimes;

            this.antsCount = antsCount;
            this.iterations = maxIterations;
            this.evaporationRate = evaporationRate;
            this.alpha = alpha;
            this.beta = beta;

            Initialize();
        }

        private void Initialize()
        {
            pheromones = new double[jobsCount][];
            heuristicValues = new double[jobsCount][];
            probabilities = new double[jobsCount][];
            schedules = new int[antsCount][];

            BestProfit = 0;
            BestSchedule = new int[jobsCount];

        }



        public void Run()
        {
            // Inicjalizacja feromonów i wartości heurystycznych
            for (int i = 0; i < jobsCount; i++)
            {
                pheromones[i] = new double[jobsCount];
                heuristicValues[i] = new double[jobsCount];
                probabilities[i] = new double[jobsCount];
                for (int j = 0; j < jobsCount; j++)
                {
                    pheromones[i][j] = 1.0 / (double)jobsCount;
                    heuristicValues[i][j] = profits[j] / (double)taskTimes[j];
                }
            }

            // Iteracje algorytmu mrówkowego
            for (int iter = 1; iter <= iterations; iter++)
            {
 
                for (int ant = 0; ant < antsCount; ant++)
                {
                    schedules[ant] = GenerateSchedule();
                }


                for (int i = 0; i < jobsCount; i++)
                {
                    for (int j = 0; j < jobsCount; j++)
                    {
                        double deltaPheromone = 0;
                        for (int ant = 0; ant < antsCount; ant++)
                        {
                            if (schedules[ant][i] == j)
                            {
                                deltaPheromone += profits[j] / (double)taskTimes[j];
                            }
                        }
                        pheromones[i][j] = (1 - evaporationRate) * pheromones[i][j] + deltaPheromone;
                    }
                }

                // Znajdź najlepszy harmonogram
                for (int ant = 0; ant < antsCount; ant++)
                {
                    int profit = CalculateProfit(schedules[ant]);
                    if (profit > BestProfit)
                    {
                        BestProfit = profit;
                        Array.Copy(schedules[ant], BestSchedule, jobsCount);
                    }
                }
            }
        }
        
        private int[] GenerateSchedule()
        {
     
            int[] schedule = new int[jobsCount];
            bool[] isUsed = new bool[jobsCount];
            int currentTime = 0;

            for (int t = 0; t < jobsCount; t++)
            {
                // Oblicz prawdopodobieństwa wyboru zadań
                double sum = 0;
                for (int j = 0; j < jobsCount; j++)
                {
                    if (!isUsed[j] && currentTime + taskTimes[j] <= deadlines[j])
                    {
                        probabilities[t][j] = Math.Pow(pheromones[t][j], alpha) * Math.Pow(heuristicValues[t][j], beta);
                        sum += probabilities[t][j];
                    }
                    else 
                        probabilities[t][j] = 0;

                }

                // Wybierz zadanie zgodnie z prawdopodobieństwami
                double r = new Random().NextDouble() * sum;
                sum = 0;
                for (int j = 0; j < jobsCount; j++)
                {
                    if (probabilities[t][j] > 0)
                    {
                        sum += probabilities[t][j];
                        if (sum >= r)
                        {
                            schedule[t] = j;
                            isUsed[j] = true;
                            currentTime += taskTimes[j];
                            break;
                        }
                    }
                }
            }

            return schedule;
        }

        // Oblicz zysk dla danego harmonogramu
        private int CalculateProfit(int[] schedule)
        {
            int profit = 0;
            int currentTime = 0;
            for (int t = 0; t < jobsCount; t++)
            {
                int j = schedule[t];
                if (currentTime + taskTimes[j] <= deadlines[j])
                {
                    profit += profits[j];
                    currentTime += taskTimes[j];
                }
            }
            return profit;
        }


    }

}
