using JobSequencingProblemACO;


/* PRoblematyczny przykład... zwraca dobrą kolejność ale zły zysk
int[] profits = { 20,10,40,30 };
int[] taskTimes = { 1,1,1,1};
int[] deadlines = { 4,1 ,1,1};
*/

int[] profits = { 100,19,27,25,15 };
int[] taskTimes = { 1, 1, 1, 1,1 };
int[] deadlines = { 2,1,2,1,3 };
ACO aco = new ACO(deadlines, profits, taskTimes);
aco.Run();


int[] bestSchedule = aco.BestSchedule;
int bestProfit = aco.BestProfit;
Console.WriteLine("Najlepszy harmonogram: " + string.Join(",", bestSchedule));
Console.WriteLine("Zysk: " + bestProfit);
