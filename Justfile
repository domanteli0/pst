jenkins:
    java -Dhudson.plugins.git.GitSCM.ALLOW_LOCAL_CHECKOUT=true \
          -jar /opt/homebrew/opt/jenkins/libexec/jenkins.war