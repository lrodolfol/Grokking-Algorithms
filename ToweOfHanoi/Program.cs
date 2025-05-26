using ToweOfHanoi;

Console.WriteLine("Jogo das Torres de Hanói");
Console.WriteLine("Aperte qualquer tecla para iniciar...");
//Console.ReadKey();

int disks = 0;
do
{
    Console.Clear();
    Console.WriteLine("Quantas torres você quer jogar? (3 a 8)");
    //disks = Convert.ToInt16(Console.ReadLine());
    disks = 4;
} while (disks < 3 || disks > 8);

new Game(disks).Run();