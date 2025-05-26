using ToweOfHanoi;

Console.WriteLine("Jogo das Torres de Hanói");
Console.WriteLine("Aperte qualquer tecla para iniciar...");
Console.ReadKey();

var disks = 0;
var repeat = 20;
List<Game> games = new List<Game>();

do
{
    Console.Clear();
    Console.WriteLine("Quantas torres você quer jogar? (3 a 8)");
    Console.WriteLine($"Será realizado {repeat} jogos de cada vez.");
    
    disks = Convert.ToInt16(Console.ReadLine());
} while (disks < 3 || disks > 8);

for (int i = 0; i < repeat; i++)
    games.Add(new Game(disks));

games.ForEach(game => game.Run());

var minAttempts = games.Min(g => g.Attemps);
Console.WriteLine($"O menor numero de tentativas com {disks} foi de: {minAttempts} jogadas.");