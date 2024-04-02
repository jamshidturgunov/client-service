pipeline {
    agent any
    stages {
       stage('Build') {
          steps {
              echo 'Runnig build automation'
      }
    }
    stage('Build Docker Image') {
        when {
           branch 'hotfix'
        }
        steps {
           script {
               app = docker.build("itarca/client-requests")
               app.inside {
                    sh 'echo $(curl localhost:7777)'
          }
        }
      }
    }
    stage('Push Docker Image') {
      when {
           branch 'hotfix'
      }
      steps {
         script {
                docker.withRegistry('https://registry.hub.docker.com','dockerhub-login'){
                        app.push("${env.BUILD_NUMBER}")
                        app.push("latest")
          }
        }
      }
    }
    stage('DeployToProduction'){
        when{
                branch 'hotfix'
        }
        steps{
                input 'Deploy to Production'
                milestone(1)
                withCredentials([usernamePassword(credentialsId:'deploy_login', usernameVariable: 'USERNAME', passwordVariable:'USERPASS')]){
                        script {
                                sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker pull itarca/client-requests:${env.BUILD_NUMBER}\""
                                try {
                                        sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker stop train-schedule\""
                                        sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker rm train-schedule\""
                                } catch (err) {
                                        echo: 'caught error: $err'
                                }
                                 sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker run --restart always --name client-service -p 7777:7777 -d itarca/client-requests:${env.BUILD_NUMBER}\""

          }
        }
      }
    }
  }
}
