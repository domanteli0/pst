pipeline {
    agent { dockerfile true }
    stages {
        stage('Build') {
            steps {
                sh 'dotnet build lab1'
            }
        }
        stage('Test') {
            steps {
                sh 'dotnet test lab1'
            }
        }
    }
}
