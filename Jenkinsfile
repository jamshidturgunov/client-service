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
           branch 'main'
        }
        steps {
           script {
               app = docker.build("jamshidturgunov/client-requests")
               app.inside {
                    sh 'echo $(curl localhost:7777)'
          }
        }
      }
    }
    stage('Push Docker Image') {
      when {
           branch 'main'
      }
      steps {
         script {
                docker.withRegistry('https://registry.hub.docker.com','dockerhub_login'){
                        app.push("${env.BUILD_NUMBER}")
                        app.push("latest")
          }
        }
      }
    }
    stage('DeployToProduction'){
        when{
                branch 'main'
        }
        steps{
                input 'Deploy to Production'
                milestone(1)
                withCredentials([usernamePassword(credentialsId:'deploy_id', usernameVariable: 'USERNAME', passwordVariable:'USERPASS')]){
                        script {
                                sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker pull jamshidturgunov/client-requests:${env.BUILD_NUMBER}\""
                                try {
                                        sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker stop client-requests\""
                                        sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker rm client-requests\""
                                } catch (err) {
                                        echo: 'caught error: $err'
                                }
                                 sh "sshpass -p '$USERPASS' -v ssh -o StrictHostKeyChecking=no $USERNAME@$stage_ip \"docker run --restart always --name client-service -p 7777:7777 -d jamshidturgunov/client-requests:${env.BUILD_NUMBER}\""

          }
        }
      }
    }
  }
}
