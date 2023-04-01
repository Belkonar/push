workspace "Push" "A fun CI/CD tool" {
    !docs docs

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
                tags "HTTP"

                perspectives {
                    InfoSec "We really need to make sure this never is exposed to open internet"
                }

                technology "dotnet web api"
            }
            
            schedulerContainer = container "scheduler" {
                component "pending service"
                component "other services"

                tags "Requires Disk"

                technology "dotnet worker"
            }
            
            nomadHost = container "nomad host" {
                tags "HTTP"
                tags "Requires Disk"

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
                tags "HTTP"

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

        apiContainer -> database "Uses" {
            tags "logical"
        }

        policyContainer -> policyDatabase "Uses" {
            tags "logical"
        }

        schedulerContainer -> apiContainer "Finds Jobs w/Statuses That Need Work"

        schedulerContainer -> vcs "Pulls Code From"
        apiContainer -> vcs "Queries References"

        schedulerContainer -> nomadHost "Schedules Jobs"
        nomadHost -> nomadClient "Places"
        nomadClient -> remoteDocker "Uses"
        nomadClient -> apiContainer "Uses"

        schedulerContainer -> artifactStorage "Writes"
        nomadClient -> artifactStorage "Uses"
        
        push = deploymentEnvironment Live {
            deploymentNode vpc {
                tags = "Amazon Web Services - Virtual Private Cloud"
                
                deploymentNode Fargate {
                    tags "Amazon Web Services - Fargate"
                    
                    policyInstance = containerInstance policyContainer {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                    
                    containerInstance schedulerContainer {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                    
                    apiInstance = containerInstance apiContainer {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                    
                    containerInstance nomadHost {
                        tags "Amazon Web Services - Elastic Container Service Service"
                    }
                }
                
                deploymentNode EC2 {
                    tags "Amazon Web Services - EC2"

                    deploymentNode "Nomad Client ASG" {
                        tags "Amazon Web Services - EC2 Auto Scaling"

                        containerInstance nomadClient {
                            tags "Amazon Web Services - EC2 Instance"
                        }

                        instances "3..N"
                    }
                    
                    deploymentNode "Remote Docker ASG" {
                        tags "Amazon Web Services - EC2 Auto Scaling"

                        containerInstance remoteDocker {
                            tags "Amazon Web Services - EC2 Instance"
                        }

                        instances "3..N"
                    }
                }

                atlas = infrastructureNode "Atlas Privatelink" {
                    tags = "Amazon Web Services - PrivateLink"   
                }
            }

            deploymentNode "Atlas VPC" {
                tags "Amazon Web Services - Virtual Private Cloud"

                atlasCluster = deploymentNode "Atlas Cluster" {
                    databaseInstance = containerInstance database {
                        tags "Amazon Web Services - DocumentDB"
                    }
                    
                    policyDatabaseInstance = containerInstance policyDatabase {
                        tags "Amazon Web Services - DocumentDB"
                    }
                }

                atlasEndpoint = infrastructureNode "Atlas VPC Endpoint" {
                    tags = "Amazon Web Services - VPC Endpoints"   
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

            policyInstance -> atlas "For Policy Database"
            apiInstance -> atlas "For App Database"

            atlas -> atlasEndpoint

            atlasEndpoint -> atlasCluster
        }
    }

    views {
        properties {
            "structurizr.sort" "created"
        }

        systemLandscape {
            include *
            autoLayout
        }

        container policyEngine {
            include *
            autoLayout
        }

        container pushSystem {
            include *
            autoLayout
        }

        deployment * Live {
            include *
            exclude relationship.tag==logical
            autoLayout
            default
        }
        
        styles {
            element "Element" {
                strokeWidth 4
            }

            element "Software System" {
                background #1168bd
                color #ffffff
            }
            
            element "Container" {
                background #ffffff
                stroke #000000
            }
            element "Infrastructure Node" {
                background #ffffff
            }
            
            element "Person" {
                shape person
                background #08427b
                color #ffffff
            }

            relationship "Relationship" {
                routing Curved
                thickness 3
                color black
            }

            element "HTTP" {
                border dashed
            }
        }

        themes https://static.structurizr.com/themes/amazon-web-services-2023.01.31/theme.json
    }
    
}
