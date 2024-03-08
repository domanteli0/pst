pipeline {
    agent { dockerfile true }
    stages {
        stage('Build') {
            steps {
                sh 'dotnet build lab4/lab4.csproj'
            }
        }
        stage('Test') {
            steps {
                sh 'dotnet test lab4/lab4.csproj'
            }
        }
    }
}
