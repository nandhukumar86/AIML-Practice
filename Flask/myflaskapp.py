from flask import Flask, render_template, request

from werkzeug.utils import secure_filename
from werkzeug.datastructures import  FileStorage


import numpy as np
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import OneHotEncoder
import tensorflow as tf
from tensorflow.keras import Sequential
from tensorflow.keras.layers import Dense
from tensorflow.keras.layers import BatchNormalization
from tensorflow.keras.optimizers import SGD
import pickle

app = Flask(__name__)

df = pd.read_csv('Signal.csv')
columnsAll = df.columns
columny = columnsAll[-1]
columnsX = columnsAll[0:-1]

X_train, X_test, y_train, y_test = train_test_split(df[columnsX], df[columny], test_size=0.30, random_state=100)



ohe = OneHotEncoder()
y_train_ohe = np.array(ohe.fit_transform(np.array(y_train).reshape(-1,1)).todense())
y_test_ohe = np.array(ohe.fit_transform(np.array(y_test).reshape(-1,1)).todense())

@app.route("/")
def hello():
    return "Hello World!"

@app.route('/uploader', methods = ['POST'])
def uploader():
   if request.method == 'POST':
      f = request.files['file']
      f.save(secure_filename(f.filename))
      return 'file uploaded successfully'

@app.route('/regressor', methods = ['POST'])
def regressor():
    model = Sequential()
    # Layer to normalize the data, with input shape 11
    model.add(BatchNormalization(input_shape=(11,))) 
    model.add(Dense(10, activation='relu', kernel_initializer='he_normal'))
    model.add(Dense(12, activation='relu', kernel_initializer='he_normal'))
    # Six layers of output for classification problem
    model.add(Dense(6, activation='softmax'))
    opt = SGD(learning_rate=0.01, momentum=0.9)
    model.compile(optimizer=opt, loss='categorical_crossentropy', metrics=['accuracy'])
    model.fit(X_train, y_train_ohe, epochs=50, batch_size=100, verbose=0) 

    dbfile = open("regressorModel.pkl", "wb" )
    pickle.dump(model, dbfile )
    dbfile.close()
    
    return "Model Ran & Pickled"

@app.route('/classifier', methods = ['POST'])
def classifier():
    model2 = Sequential()

    # Layer to normalize the data, with input shape 11
    model2.add(BatchNormalization(input_shape=(11,))) 
    model2.add(Dense(10, activation='relu', kernel_initializer='he_normal'))
    model2.add(Dense(12, activation='relu', kernel_initializer='he_normal'))
    # one layer of output for regression problem
    model2.add(Dense(1))

    opt = SGD(learning_rate=0.01, momentum=0.9)

    model2.compile(optimizer = opt, loss='mse', metrics=['mse', 'mae'])
    dbfile = open("classfierModel.pkl", "wb" )
    pickle.dump(model2, dbfile )
    dbfile.close()
    
    return "Model Ran & Pickled"

if __name__ == '__main__':
    app.run(debug=True)