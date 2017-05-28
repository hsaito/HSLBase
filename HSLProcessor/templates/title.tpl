<html>

<head>
    <title>Detail for {{title}}</title>
    <style>
        #content_table {
            border: solid 1px
        }

        #content {
            padding: 10px;
        }

        .cell {
            border: solid 1px;
            padding: 2px;
        }

        a {
            text-decoration: none;
            color: black;
        }

        a:link {
            text-decoration: none;
            color: black;
        }

        a:visited {
            text-decoration: none;
            color: black;
        }

        a:hover {
            text-decoration: underline;
            color: blue;
        }

        a:active {
            text-decoration-style: underline;
            color: darkgreen;
        }
    </style>
</head>

<body>
    <h1>{{title}}</h1>
    <div id="heading">
        Song details for {{title}}
    </div>
    <div id="content">
        {{content}}
    </div>
</body>

</html>