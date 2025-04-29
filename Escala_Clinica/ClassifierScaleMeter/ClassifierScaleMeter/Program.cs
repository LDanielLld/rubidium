using ClassifierScaleMeter;


//Añadir datos de entrada
var sampleData = new ScaleMeterModel.ModelInput()
{
    Col0 = "This restaurant was wonderful."
};

//Cargar modelop y predecir la salida de la muestra de datos de entrada
var result = ScaleMeterModel.Predict(sampleData);

//Si la prediccion es 1, es positivo. En otro caso es negativo
var sentiment = result.PredictedLabel == 1 ? "Positive" : "Negative";


// See https://aka.ms/new-console-template for more information
Console.WriteLine($"Text: {sampleData.Col0}\nSentiment: {sentiment}");
