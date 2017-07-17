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
            text-decoration-style: solid;
            color: blue;
        }

        a:active {
            text-decoration: underline;
            text-decoration-style: solid;
            color: darkgreen;
        }
    </style>
</head>

<body>
    <h1>{{artist}}</h1>
    <div id="heading">
        Artist details for {{artist}}
    </div>
    <hr>
    <h2>Titles by {{artist}}</h2>
    <div id="content">
        {{content}}
    </div>
    <hr>
    <a href="../">Top</a>
    <hr>
    <div>
        Generated by the <a href="https://github.com/hsaito/HSLBase">HSLBase</a> system by <a href="https://hidekisaito.com">Hideki Saito</a>.
    </div>
    <div>
        Generated at {{generated}}.
    </div>
</body>

</html>