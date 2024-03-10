pipeline {
    agent { dockerfile true }
    options {
        // Timeout counter starts AFTER agent is allocated
        timeout(time: 10, unit: 'MINUTES')
    }
    triggers { cron('*/30 * * * *') }
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
