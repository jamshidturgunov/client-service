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
	       app = docker.build("itarca/client-requests")
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
                docker.withRegistry('https://registry.hub.docker.com','dockerhub-login'){
                        app.push("${env.BUILD_NUMBER}")
                        app.push("latest")
          }
        }
      }
    }
  }
} 
