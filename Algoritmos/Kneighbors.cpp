// Algoritmos.cpp : Este arquivo contém a função 'main'. A execução do programa começa e termina ali.
//

#include <iostream>
#include <map>
#include <vector>
using namespace std;

void neighbors() {
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
	cin.get();
}

int binarySearch(int arr[], int size, int target) {
	int left = 0;
	int right = size - 1;

	while (left <= right) {
		int middle = left + (right - left) / 2;

		// Verifica se o elemento alvo está no meio
		if (arr[middle] == target)
			return middle;

		// Se o elemento alvo for maior, ignore a metade esquerda
		if (arr[middle] < target)
			left = middle + 1;
		// Se o elemento alvo for menor, ignore a metade direita
		else
			right = middle - 1;
	}

	// Retorna -1 se o elemento não for encontrado
	return -1;
}
int recurssion(int number) {
	//permutation use stack, then your use is expensive and use memory a lot. Prefer use loop if is possible
	int result = number;

	if (number == 1) {
		return number;
	}

	while (number > 1) {
		number--;
		result = result * number;
		recurssion(number);
	}

	return result;
}
int recurssionTwo(int number) {
	//permutation use stack, then your use is expensive and use memory a lot. Prefer use loop if is possible
	int result = number;
	if (number == 1) {
		return number;
	}
	while (number > 1) {
		number--;
		result = result * number;
		recurssion(number);
	}
	return result;
}
void divideToConquer(int horizontal, int vertical) {
	do{
		if (horizontal == vertical) {
			cout << "The MDC is " << horizontal << endl;
			return;
		}

		if (horizontal > vertical) {
			horizontal -= vertical;
		}
		else {
			vertical -= horizontal;
		}
	} while (horizontal > 0 && vertical > 0);

	cout << "It's not possible to find the MDC " << endl;
}
vector<int> quickSort(vector<int>& arr) { //with errors
	
	if (arr.size() <= 1) {
		return arr;
	}

	int numPivot = arr.size() > 2 ? 2 : 1;
	int pivot = arr[numPivot];
	vector<int> left;
	vector<int> right;

	for (int i = 0; i < arr.size(); i = i + 1) {
		if (arr[i] <= pivot) {
			left.push_back(arr[i]);
		}
		else if (arr[i] > pivot) {
			right.push_back(arr[i]);
		}
	}

	vector<int> dd = quickSort(left); 
	dd.push_back(pivot);
	vector<int> cc = quickSort(right);
	dd.insert(dd.end(), cc.begin(), cc.end()); // Fix: use insert instead of push_back

	return dd; // Fix: return the sorted vector
}


int main()
{
	//neighbors();

	//int perm = recurssion(9);
	//cout << "The permutation is " << perm << endl;

	//int target = binarySearch(new int[10] { 1,3,5,7,9 }, 5, 7);
	//cout << "The target is in the position " << target << endl;

	//divideToConquer(1680, 10202);

	vector<int> vec = { 1,8,3,5,7,9,2 };
	quickSort(vec);

	return 0;
}
