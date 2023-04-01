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
        thickness 3
        color black
    }

    element "HTTP" {
        border dashed
    }

    element "MongoDB" {
        stroke #00684a
        color #00684a
        icon "mongo.png"
    }

    element "Atlas" {
        stroke #001e2b
        color #001e2b
        icon atlas.png
    }
}