// Algoritmos.cpp : Este arquivo contém a função 'main'. A execução do programa começa e termina ali.
//

#include <iostream>
#include <map>
using namespace std;

int main()
{
	srand(static_cast<unsigned int>(time(0)));
	string users[10] = { "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Hank", "Ivy", "Jack" };
	string categories[9] = { "comedy","action","fiction", "fantasy","drama","horror","romance","adventure","documentary" };

	int minValue = 1, maxValue = 5, randValue = 0, distanceNumers, distance = 0;

	int usersPreferencies[size(users)][size(categories)];

	for (int i = 0; i < size(users); i = i + 1) {

		for (int y = 0; y < size(categories); y = y + 1) {

			randValue = (rand() % 5) + 1;
			usersPreferencies[i][y] = randValue;

			cout << "The " << categories[y] << " avaliation from " << users[i] << " is " << randValue << endl;
		}

		cout << "=============" << endl;
	}


	cout << endl << endl << "Find the distance - K-neighbors" << endl << endl;

	for (int i = 0; i < size(users) - 1; i = i + 1) {
		distance = 0;

		for (int y = i + 1; y < size(users); y = y + 1) {
			if (i == y) continue;

			//get distance or each categorie. The vote from user[i] to category[z] and The vote from user[y] to category[z] 
			//FORMULE FIND DISTANCE: (    (X1 - X2)²  + (Y1 - Y2)² + (Z1 - Z2)² .....     )¹/²
			for (int z = 0; z < size(categories); z = z + 1) {
				int value = (usersPreferencies[i][z] - usersPreferencies[y][z]);
				distance = distance + pow((value * value), 0.5);
			}

			cout << "The distance between " << users[i] << " and " << users[y] << " is " << distance << endl;
		}
	}

	cin.get();
}
