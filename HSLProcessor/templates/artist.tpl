<html>

<head>
    <title>Detail for {{artist}}</title>
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
    <h1>{{artist}}</h1>
    <div id="heading">
        Artist details for {{artist}}
    </div>
    <h2>Titles by {{artist}}</h2>
    <div id="content">
        {{content}}
    </div>
</body>

</html>