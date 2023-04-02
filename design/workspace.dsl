workspace "Push" "A fun CI/CD tool" {
    !docs docs

    model {
        user = person Actor
        
        pushSystem = softwareSystem "Push" "CI/CD Deployment Tool" {
            ui = container UI {
                tags "spa"

                technology "dotnet blazor"
            }

            artifactStorage = container "Artifact Storage" {
                perspectives {
                    InfoSec "The S3 bucket here will have a lifecycle policy to purge the contents as well as keep the artifacts encrypted"
                }
            }
        
            apiContainer = container "API" {
                tags "HTTP"

                perspectives {
                    InfoSec "We really need to make sure this never is exposed to open internet"
                }

                technology "dotnet web api"
            }
            
            schedulerContainer = container "Scheduler" {
                component "pending service"
                component "other services"

                tags "Requires Disk"

                technology "dotnet worker"
            }
            
            nomadHost = container "Nomad Host" {
                tags "HTTP"
                tags "Requires Disk"

                technology "nomad agent"
            }
            
            nomadClient = container "Nomad Client" {
                technology "nomad agent + dotnet cli"
            }
            
            remoteDocker = container "Remote Docker Host" {
                perspectives {
                    InfoSec "SSH key management still needs to be figured out."
                }

                technology "docker"
            }
            
            database = container "App Database" {
                technology "MongoDB"
            }

            policyContainer = container "Policy Engine API" {
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

        vcs = softwareSystem VCS "Github Based Version Control"
                
        user -> ui "Uses"
        user -> apiContainer "Uses" "Intranet"
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
                    
                    deploymentNode "Policy Service" {
                        tags "Amazon Web Services - Elastic Container Service Service"

                        policyInstance = containerInstance policyContainer {
                            tags "Amazon Web Services - Elastic Container Service Task"
                        }

                        instances "3"
                    }
                    
                    deploymentNode "Scheduler Service" {
                        tags "Amazon Web Services - Elastic Container Service Service"

                        containerInstance schedulerContainer {
                            tags "Amazon Web Services - Elastic Container Service Task"
                        }

                        instances "1"
                    }

                    deploymentNode "API Service" {
                        tags "Amazon Web Services - Elastic Container Service Service"

                        apiInstance = containerInstance apiContainer {
                            tags "Amazon Web Services - Elastic Container Service Task"
                        }

                        instances "3"
                    }

                    deploymentNode "Nomad Service" {
                        tags "Amazon Web Services - Elastic Container Service Service"

                        containerInstance nomadHost {
                            tags "Amazon Web Services - Elastic Container Service Task"
                        }

                        instances "3"
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
                    tags "Atlas"

                    databaseInstance = containerInstance database {
                        tags "MongoDB"
                    }
                    
                    policyDatabaseInstance = containerInstance policyDatabase {
                        tags "MongoDB"
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

        systemLandscape "landscape" {
            include *
        }

        container pushSystem "push-container" {
            include *
            exclude element.tag==spa
        }

        deployment * Live "main-deployment" {
            include *
            exclude relationship.tag==logical
            default
        }
        
        !include "helpers/theme.dsl"

        themes https://static.structurizr.com/themes/amazon-web-services-2023.01.31/theme.json
    }
    
}
