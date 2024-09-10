using ConsoleUserInteractionHelper;

var helper = new ConsoleHelper();

var taskToWait = Task.Delay(4400);
helper.ShowSpinnerUntilTaskIsRunning(taskToWait);