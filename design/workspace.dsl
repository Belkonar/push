workspace "Push" "A fun CI/CD tool" {

    model {
        user = person Actor
        
        pushSystem = softwareSystem "Push" "CI/CD Deployment Tool" {
            ui = container UI {
                technology "dotnet blazor"
            }

            artifactStorage = container "Artifact Storage" {
                perspectives {
                    InfoSec "The S3 bucket here will have a lifecycle policy to purge the contents as well as keep the artifacts encrypted"
                }
            }
        
            apiContainer = container "api" {
                perspectives {
                    InfoSec "We really need to make sure this never is exposed to open internet"
                }

                technology "dotnet web api"
            }
            
            schedulerContainer = container "scheduler" {
                component "pending service"
                component "other services"

                technology "dotnet worker"
            }
            
            nomadHost = container "nomad host" {
                technology "nomad agent"
            }
            
            nomadClient = container "nomad client" {
                technology "nomad agent + dotnet cli"
            }
            
            remoteDocker = container "remote docker" {
                perspectives {
                    InfoSec "SSH key management still needs to be figured out."
                }

                technology "docker"
            }
            
            database = container "App Database" {
                technology "MongoDB"
            }
        }

        vcs = softwareSystem VCS "Github Based Version Control"
        
        policyEngine = softwareSystem "Policy Engine" "Document Based Authorization" {
            policyContainer = container "policy-engine" {
                component "policy management"

                perspectives {
                    InfoSec "This service will be hard locked to the API via SG."
                }

                technology "nestjs"
            }

            policyDatabase = container "Policy Database" {
                perspectives {
                    InfoSec "This will have a dedicated set of credentials for this database."
                }
                technology "MongoDB"
            }
        }
        
        pushSystem -> policyEngine
        
        user -> ui "Uses" 
        user -> apiContainer "Uses Directly" "Intranet"
        ui -> apiContainer "Uses" "Intranet"
        
        apiContainer -> policyContainer "Uses"
        apiContainer -> database "Uses"
        policyContainer -> policyDatabase "Uses"
        schedulerContainer -> apiContainer "Calls"

        schedulerContainer -> vcs "Pulls From"

        schedulerContainer -> nomadHost "Schedules Jobs"
        nomadHost -> nomadClient "Places"
        nomadClient -> remoteDocker "Uses"
        nomadClient -> apiContainer "Uses"

        schedulerContainer -> artifactStorage "Writes"
        nomadClient -> artifactStorage "Uses"
        
        push = deploymentEnvironment Live {
            deploymentNode vpc {
                tags = "Amazon Web Services - Virtual Private Cloud"
                
                atlas = deploymentNode "Atlas Privatelink" {
                    tags = "Amazon Web Services - PrivateLink"
                    
                    containerInstance database {
                        tags "Amazon Web Services - DocumentDB"
                    }
                    
                    containerInstance policyDatabase {
                        tags "Amazon Web Services - DocumentDB"
                    }
                }
                
                deploymentNode Fargate {
                    tags "Amazon Web Services - Fargate"
                    
                    containerInstance policyContainer {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                    
                    containerInstance schedulerContainer {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                    
                    containerInstance apiContainer {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                    
                    containerInstance nomadHost {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                }
                
                deploymentNode EC2 {
                    tags "Amazon Web Services - EC2"
                    containerInstance nomadClient {
                        tags "Amazon Web Services - EC2 Instance"
                    }
                    
                    containerInstance remoteDocker {
                        tags "Amazon Web Services - EC2 Instance"
                    }
                }
            }
            
            deploymentNode S3 {
                tags "Amazon Web Services - Simple Storage Service"
                
                containerInstance ui {
                    tags "Amazon Web Services - Simple Storage Service Bucket"
                }

                containerInstance artifactStorage {
                    tags "Amazon Web Services - Simple Storage Service Bucket"
                }
            }
        }
    }

    views {
        deployment * Live {
            include *
            autoLayout
        }
        
        systemContext pushSystem {
            include *
            autoLayout
        }

        systemContext policyEngine {
            include *
            autoLayout
        }

        container pushSystem {
            include *
            autoLayout
        }
        
        container policyEngine {
            include *
            autoLayout
        }
        
        styles {
            element "Software System" {
                background #1168bd
                color #ffffff
            }
            element "Container" {
                background #ffffff
            }
            element "Infrastructure Node" {
                background #ffffff
            }
            
            element "Person" {
                shape person
                background #08427b
                color #ffffff
            }
        }
        themes https://static.structurizr.com/themes/amazon-web-services-2023.01.31/theme.json
    }
    
}