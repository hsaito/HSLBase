<html>

<head>
    <title>Detail for {{source}}</title>
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
    <h1>{{source}}</h1>
    <div id="heading">
        Source details for {{source}}
    </div>
    <h2>Titles of {{source}}</h2>
    <div id="content">
        {{content}}
    </div>
</body>

</html>