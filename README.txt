Made by:
Davis Pham
Anthony Wong

Repository link: https://github.com/Minitte/ozma-net

Completed:
Numbers - ~90% accuracy

Attempted:
Alphabet - ~75% accuracy
Random objects - ~10% accuracy (Equivalent to random guessing)

Projects:

console - Used for training data. Wont work because training data was not included in this zip to reduce size.
mnist_data_creator - Used for converting regular images into mnist data format with the folders being the category.
ozmanet - Neural network implementation
wpf-ozma-net - Application with drawing UI. Weights to load in can be found in the weights folder.

What we learned:

Basics of feed forward and backpropagation in a neural network.

Preprocessing the drawing input is important for getting an accurate output from the neural network.
Since we didn't scale or crop the drawings to be exactly the same as the training data, we got incorrect output when drawing in certain ways.

Reducing the quality of the drawing image doesn't affect the accuracy very much.

