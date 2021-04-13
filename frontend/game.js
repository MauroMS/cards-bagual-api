$(document).ready(function() {

    let baseURL = 'https://bagualapi.azurewebsites.net/Shithead';
    //let baseURL = 'http://localhost:5000/Shithead';

    let imageURL = 'https://deckofcardsapi.com/static/img/XX.png';

    let imageBackURL = "https://cdn.shopify.com/s/files/1/0200/7616/products/cardback.png?v=1523371937";

    let gameName = ""; 

    let playerName = ""; 


    let myTurn = false;

    let playerId;

    let startMode = true;
    let globalGameStatus = "SETUP";

    let errorCount = 0;

    let touchTargetElementTable = false;
    let touchInsteadOfClick = false;

    //  init alles
    let refresher = null;

    gameNamesDataList();

    let gameNamesRefresher = setInterval(gameNamesDataList, 2000);;

    function gameNamesDataList() {
        fetch(baseURL + '/game').then(function(response) {

            if (response.status != 200) {
                writeResponseError(response);
                return;
            }

            response.json().then(function(games) {

                var options = games.map(function(game) {
                    return `<option value="${game.name}">Created by '${game.createdBy}' - ${game.playersCount} Players</option>`;
                }).join("");


                if (document.getElementById("gameNames").innerHTML != options) {
                    document.getElementById("gameNames").innerHTML = options;
                }
            });
        });
    }

    function getImageUrl(card) {
        return imageURL.replace("XX", card);
    }

    function initializePlayer(playerIndex, player) {
        var pIndex = "p" + (playerIndex);

        $("#" + pIndex).removeClass("hidden");

        if (player.status == "SETUP") {

            if (!($("#" + pIndex).hasClass("playerOnSetup"))) {
                $("#" + pIndex).addClass("playerOnSetup");
            }

        } else {
            $("#" + pIndex).removeClass("playerOnSetup");
        }

        document.getElementById(pIndex + "name").innerHTML = player.name;
    }

    function highlightTurn(playerTurn) {

        if (document.getElementById("myname").innerHTML == playerTurn) {
            myTurn = true;
            $("#myname").addClass("currentPlayer");

            for (var i = 1; i < 5; i++) {
                $("#p" + (i) + "name").removeClass("currentPlayer");
            }

            var containerCheck = $("#myCards IMG").length > 0 ?
                                  "myCards" : $("#myCardsUp IMG").length > 0 ?
                                    "myCardsUp": "myCardsDown";  

            var canPlaceCard = false;
            if (containerCheck != "myCardsDown") {
                var allParentCards = document.getElementById(containerCheck).children;                        

                for (var i = 0; i < allParentCards.length; i++) {
                    var anotherCard = allParentCards[i];
                    var cardVal = getCardValueAndSuit(anotherCard.src);
                    if (canCardGoOnMesa(cardVal)){
                        canPlaceCard = true;
                        break;
                    }
                }
            } else {
                canPlaceCard = true;
            }

            if (canPlaceCard) {
                $("#discard").removeClass("hidden");
            } else {
                $("#pickTable").removeClass("hidden");
            }

        } else {
            $("#discard").addClass("hidden");
            $("#pickTable").addClass("hidden");
            $("#myname").removeClass("currentPlayer");
            myTurn = false;

            for (var i = 1; i < 5; i++) {
                if (document.getElementById("p" + (i) + "name").innerHTML == playerTurn) {
                    $("#p" + (i) + "name").addClass("currentPlayer");
                } else {
                    $("#p" + (i) + "name").removeClass("currentPlayer");
                }
            }
        }
    }

    function buildCardImages(pileList, classes, draggable, lastDownCard) {
        var cardsList = "";

        var someCardWidth = 40;

        if (document.getElementById("mesaCards").children.length > 0)  {
            someCardWidth = document.getElementById("mesaCards").children[0].offsetWidth;
        } else if (document.getElementById("myCardsDown").children.length > 0)  {
            someCardWidth = document.getElementById("myCardsDown").children[0].offsetWidth;
        }

        // 1.7 >> cards are 70% visible
        var threshold = Math.floor(document.getElementById("mesa").offsetWidth / (someCardWidth / 1.7));

        for (var i = 0; i < pileList.length; i++) {
            var styleCss;

            if (i >= 2 * threshold) {
                styleCss = `grid-column-start: ${i+1-(threshold*2)};grid-column-end: ${i+3-(threshold*2)};grid-row-start:3;grid-row-end:5;`;
            } else if (i >= threshold) {
                styleCss = `grid-column-start: ${i+1-threshold};grid-column-end: ${i+3-threshold};grid-row-start:2;grid-row-end:4;`;
            } else {
                styleCss = `grid-column-start: ${i+1};grid-column-end: ${i+3}`;
            }

            var classForCard = classes;

            if (lastDownCard == pileList[i]) {
                classForCard += " lastDown";
            }

            cardsList += `<img class="carta ${classForCard}" draggable="${draggable}" style="${styleCss}" alt="${pileList[i]}" src="${getImageUrl(pileList[i])}" />`;
        }
        return cardsList;
    }

    function checkIfIAmShithead(gameStatus, player) {

        if (gameStatus == "OUT") {
            clearInterval(refresher);

            if (player.status == "PLAYING") {

                document.getElementById("myCardsDown").innerHTML = buildCardImages(player.downCards, "mycarta", false);

                document.getElementById("shitheadAudio").play();
                alert("You are the Shithead!!!");

            } else {
                alert("The game is over!!!");
            }
            setTimeout(function(){ location.reload(); }, 10000);
        }
    }


    function refreshBurned(burnedCardsCount, lastBurnedCard) {

        document.getElementById("burnedCount").innerHTML = burnedCardsCount;

        if (lastBurnedCard != null) {
            if (document.getElementById("burnedCards").innerHTML.indexOf(lastBurnedCard) == -1) {
                card = `<img class="carta" draggable="false" style="left: 20px" src="${getImageUrl(lastBurnedCard)}"/>`;
                document.getElementById("burnedCards").innerHTML = card;
                $("#burned").addClass("burnedLastTime");
            } else {
                $("#burned").removeClass("burnedLastTime");
            }

        }
    }

    function refreshDeck(remaining) {
        document.getElementById("deckCount").innerHTML = remaining;

        if (remaining == 0) {
            document.getElementById("deckCards").innerHTML = "";
        }
    }

    function refreshPlayers(players) {
        for (var i = 0; i < players.length; i++) {
            initializePlayer(i + 1, players[i]);
            refreshPlayer(i + 1, players[i]);
        }
    }

    function refreshPlayer(playerIndex, player) {

        // compare incoming data to existent

        var currentCards = $("#p" + playerIndex + "CardsUp IMG").map(function(i, imgCard) {
            return getCardValueAndSuit(imgCard.src);
        }).get().join(",");

        var apiCards = player.openCards.join(",");

        if (currentCards != apiCards) {
            document.getElementById("p" + playerIndex + "CardsUp").innerHTML = buildCardImages(player.openCards, "", false)
        }

        while ($("#p" + playerIndex + "CardsDown IMG").length > player.downCount) {
            ($("#p" + playerIndex + "CardsDown IMG")[0]).remove();
        }

        var cntHandPlayerCards = player.handCount;
        document.getElementById("p" + playerIndex + "Count").innerHTML = cntHandPlayerCards;


        if (player.status == "OUT") {
            $("#p" + playerIndex + "name").addClass("playerOut");
        }

    }


    function refreshTable(tableCards) {

        // compare incoming data to existent
        var currentCards = $("#mesaCards IMG").map(function(i, imgCard) {
            return getCardValueAndSuit(imgCard.src);
        }).get().join(",");

        var apiCards = tableCards.join(",");

        if (currentCards != apiCards) {
            document.getElementById("mesaCards").innerHTML = buildCardImages(tableCards, "", false);
            document.getElementById("mesaCardsCount").innerHTML = tableCards.length;
        }

    }

    function refreshInterval() {

        fetch(baseURL + '/game/' + gameName + '/' + playerId).then(function(response) {

            if (response.status != 200) {
                writeResponseError(response);
                return;
            }

            response.json().then(function(game) {
                refreshGame(game);
            });
        });
    }

    function refreshGame(game) {

        globalGameStatus = game.status;

        refreshDeck(game.deckCount);

        refreshTable(game.tableCards);

        refreshPlayers(game.players);

        refreshMyCards(game.mySelf);

        refreshBurned(game.burnedCardsCount, game.lastBurnedCard);

        highlightTurn(game.playerNameTurn);

        checkIfIAmShithead(game.status, game.mySelf);
    }


    function refreshMyCards(mySelf) {

        // for both hands and open piles 
        var hasHandCard = false;
        var hasOpenCard = false;

        for (var a = 0; a < 2; a++) {
            var elementContainer = "myCardsUp";
            var pile = "openCards";
            if (a == 1) {
                elementContainer = "myCards";
                pile = "handCards";
                hasHandCard = mySelf[pile].length > 0;
            } else {
                hasOpenCard = mySelf[pile].length > 0;
            }

            // compare incoming data to existent
            var currentCards = $("#" + elementContainer + " IMG").map(function(i, imgCard) {
                return getCardValueAndSuit(imgCard.src);
            }).get().join(",");

            var apiCards = mySelf[pile].join(",");

            if (currentCards != apiCards) {
                document.getElementById(elementContainer).innerHTML = buildCardImages(mySelf[pile], "mycarta", true, mySelf.lastDownCard);
            }
        }

        $("#myCards").css({
            zIndex: (hasHandCard ? 103 : 100)
        });
        $("#myCardsUp").css({
            zIndex: (hasHandCard ? 100 : (hasOpenCard ? 102 : 100))
        });
        $("#myCardsDown").css({
            zIndex: ((hasHandCard || hasOpenCard) ? 100 : 101)
        });

        while ($("#myCardsDown IMG").length > mySelf.downCount) {
            ($("#myCardsDown IMG")[0]).remove();
        }

        if (mySelf.status == "OUT") {
            $("#myname").addClass("playerOut");
        }

    }

    function createGameAndPlayer(gameNameInput, playerNameInput) {
        document.getElementById("myname").innerHTML = playerNameInput;
        document.getElementById("gameNameDisplay").innerHTML += gameNameInput;

        gameName = encodeURIComponent(gameNameInput.toLowerCase());
        playerName = encodeURIComponent(playerNameInput);

        fetch(baseURL + '/game/' + gameName + '/player/' + playerName).then(function(response) {

            if (response.status != 200) {
                if (response.status == 422) {
                    response.text().then(function(rb) {
                        alert("Cannot join game: " + rb);
                    });
                } else {
                    writeResponseError(response);
                }
                return;
            }

            response.json().then(function(shAPI) {
                clearInterval(gameNamesRefresher);

                playerId = shAPI.playerId;
                console.log("From SHAPI: me = " + playerId);

                refreshInterval();
                refresher = setInterval(refreshInterval, 4000);
                $('#gameStarter').addClass("hidden");
            });
        });
    }

    function getCardValueAndSuit(imgCartaSrc) {
        var pieces = imgCartaSrc.split("/");
        return pieces[pieces.length - 1].split(".")[0];
    }

    function getCardValue(imgCartaSrc) {
        var pieces = imgCartaSrc.split("/");
        return getCardCodeValue(pieces[pieces.length - 1].split(".")[0]);
    }

    function switchCardUpAndHand(cardA, cardB) {
        var cardASrc = cardA.attr("src");
        var cardBSrc = cardB.attr("src");

        var cardAParent = cardA.parent().attr("id");
        var cardBParent = cardB.parent().attr("id");

        cardA.attr("src", cardBSrc);
        cardB.attr("src", cardASrc);

        if (cardAParent == "myCardsUp") {
            fetch(baseURL + '/game/' + gameName + '/' + playerId + "/switch/" + getCardValueAndSuit(cardASrc) + "/" + getCardValueAndSuit(cardBSrc));
        } else if (cardAParent == "myCards") {
            fetch(baseURL + '/game/' + gameName + '/' + playerId + "/switch/" + getCardValueAndSuit(cardBSrc) + "/" + getCardValueAndSuit(cardASrc));
        }

    }

    function getCardCodeValue(cardCode) {
        return cardCode.substring(0, cardCode.length - 1);
    }

    function getMesaLastCardCodeValue() {
        var cards = $("#mesaCards IMG").map(function(i, imgCard) {
            return getCardValueAndSuit(imgCard.src);
        }).get();


        for (var i = cards.length - 1; i >= 0; i--) {
            var valor = getCardCodeValue(cards[i]);
            if (valor != "3") {
                return valor;
            }
        }
        return "";
    }

    function getNumericCardValue(cardCode) {
        if (cardCode == "A") return 14;
        if (cardCode == "K") return 13;
        if (cardCode == "Q") return 12;
        if (cardCode == "J") return 11;
        if (cardCode == "0") return 10;
        return parseInt(cardCode);
    }

    function canCardGoOnMesa(cardCode) {
        var mesaLastCardValue = getMesaLastCardCodeValue();

        if (mesaLastCardValue == "") {
            return true;
        }

        var cardCodeValue = getCardCodeValue(cardCode);

        if (cardCodeValue == "3" || cardCodeValue == "2" || cardCodeValue == "0") {
            return true;
        }

        if (mesaLastCardValue == cardCodeValue) {
            return true;
        }

        if (mesaLastCardValue == "7") {
            return ["4", "5", "6"].includes(cardCodeValue);
        }

        return (getNumericCardValue(cardCodeValue) > getNumericCardValue(mesaLastCardValue));
    }

    function selectCardForMesa(carta, parentId) {
        if (parentId == "myCardsDown") {
            carta.addClass("selected");
        } else {
            var cardCode = getCardValueAndSuit(carta.attr("src"));

            if (canCardGoOnMesa(cardCode)) {
                carta.addClass("selected");
            }
        }
    }


    function writeResponseError(response) {
        errorCount++;

        if (response.status == 422) {
            response.json().then(function(data) {
                console.log(data.detail);
            });
        } else {
            console.log("Error calling shithead API");
        }
        if (errorCount > 10) {
            console.log("Giving up calling API");
            clearInterval(refresher);
        }

    }

    function grabMesaCards() {
        if (document.getElementById("mesaCards").innerHTML != "") {
            fetch(baseURL + '/game/' + gameName + '/' + playerId + "/table").then(function(response) {

                if (response.status != 200) {
                    writeResponseError(response);
                    return;
                }

                response.json().then(function(data) {
                    refreshGame(data);
                });
            });
        }
    }

    function discardSelectedCard() {
        if (!myTurn || startMode) {
            return;
        }

        var goToNextPlayer = 0;

        var selectedElem = $("IMG.selected");

        var elemParent = selectedElem.parent().attr("id");

        if (selectedElem.length == 0) {

            return;
        }

        if (elemParent != "myCardsDown") {
            var cards = selectedElem.map(function(i, imgCard) {
                return getCardValueAndSuit(imgCard.src);
            }).get().join(",");

            fetch(baseURL + '/game/' + gameName + '/' + playerId + "/discard/" + cards).then(function(response) {

                if (response.status != 200) {
                    writeResponseError(response);
                    return;
                }

                response.json().then(function(data) {
                    $("IMG.selected").removeClass("selected");
                    refreshGame(data);
                });
            });

        } else {
            fetch(baseURL + '/game/' + gameName + '/' + playerId + "/discard/down").then(function(response) {

                if (response.status != 200) {
                    writeResponseError(response);
                    return;
                }

                response.json().then(function(data) {
                    $("IMG.selected").removeClass("selected");
                    refreshGame(data);
                });
            });
        }
    }

    function selectSameNumberCardsForMesa(dragCard, parentId) {
        var allParentCards = document.getElementById(parentId).children;

        var dragCardVal = getCardValue(dragCard.attr("src"));

        // never select multiple 10  or cards down
        if (dragCardVal == 0 || parentId == "myCardsDown") {
          selectCardForMesa(dragCard, parentId);
          
        } else {          
          for (var i = 0; i < allParentCards.length; i++) {
              var anotherCard = allParentCards[i];
              var cardVal = getCardValue(anotherCard.src);
              if (cardVal == dragCardVal) {
                  selectCardForMesa($(anotherCard), parentId);
              }
          }
        }
    }

    function startDragCardToTable(dragCard) {
        var parentId = dragCard.parent().attr('id');

        if (dragCard.hasClass("selected")) {
            return;
        }

        var canPlayUp = ($("#myCards IMG").length == 0);
        var canPlayDown = canPlayUp && ($("#myCardsUp IMG").length == 0);

        if (myTurn && (parentId == "myCards" || (canPlayUp && parentId == "myCardsUp") || (canPlayDown && parentId == "myCardsDown"))) {

            if ($("IMG.selected").length > 0) {
              if (parentId != "myCardsDown") {  
                  var dragCardVal = getCardValue(dragCard.attr("src"));
              
                  var alreadySelSrc = $($("IMG.selected")[0]).attr("src");
                  var alreadySelVal = getCardValue(alreadySelSrc);

                  if (dragCardVal != alreadySelVal) {
                     $("IMG.selected").removeClass("selected");
                     selectSameNumberCardsForMesa(dragCard, parentId);
                  }
              }

            } else {
                if (parentId != "myCardsDown") {
                    selectSameNumberCardsForMesa(dragCard, parentId);
                } else {
                    selectCardForMesa(dragCard, parentId);
                }
            }
        }
    }


    function handleCardClick(aCard) {
        var parentId = $(aCard).parent().attr('id');

        if ($(aCard).hasClass("selected")) {
            $(aCard).removeClass("selected");
            return;
        }

        if (startMode) {
            if ($("IMG.selected").length == 1) {
                var alreadySelectedParentId = $("IMG.selected")[0].parentElement.id;
                if (parentId != alreadySelectedParentId && parentId != "myCardsDown") {
                    switchCardUpAndHand($(aCard), $($("IMG.selected")[0]));
                    $($("IMG.selected")[0]).removeClass("selected");
                }
            } else if (parentId != "myCardsDown") {
                $(aCard).addClass("selected");
            }
        } else {
            var canPlayUp = ($("#myCards IMG").length == 0);
            var canPlayDown = canPlayUp && ($("#myCardsUp IMG").length == 0);

            if (myTurn && (parentId == "myCards" || (canPlayUp && parentId == "myCardsUp") || (canPlayDown && parentId == "myCardsDown"))) {

                if ($("IMG.selected").length == 0) {
                    selectSameNumberCardsForMesa($(aCard), parentId);
                } else {
                    if (parentId != "myCardsDown") {
                        var alreadySelSrc = $($("IMG.selected")[0]).attr("src");
                        var alreadySelVal = getCardValue(alreadySelSrc);

                        var thisSelVal = getCardValue($(aCard).attr("src"));
                        if (thisSelVal == alreadySelVal) {
                            selectCardForMesa($(aCard), parentId);
                        }
                    }
                }
            }
        }
    }


    // bindings


    $(document).on("click", "img.mycarta", function() {
        if (!touchInsteadOfClick) {
            handleCardClick(this);
        }
    });

    $('#starter').click(function() {
        startMode = false;
        $(this).addClass("hidden");
        $("IMG.selected").removeClass("selected");
        fetch(baseURL + '/game/' + gameName + '/' + playerId + "/start");
    });

    $('#discard').click(function() {
        discardSelectedCard();
    });

    $('#pickTable').click(function() {
        grabMesaCards();
    });


    $('#infoBoxTitle').click(function() {
        document.getElementById("infoBox").classList.toggle("hidden");
    });


    $('#commence').click(function() {
        var game = document.getElementById("gameName").value;
        var player = document.getElementById("playerName").value;

        $('#dataForm')[0].reportValidity();

        if (game.trim() == "" || player.trim() == "") {
            return;
        }
        createGameAndPlayer(game, player);
    });


    $(document).on("dblclick", "#mesa", function() {
        grabMesaCards();
    });

    $(document).on("click", "#shitheadIcon", function() {
        // add a bot player
        if (globalGameStatus == "SETUP"){
          if (confirm("Add Bot player to the game?") ) 
              fetch(baseURL + '/game/' + gameName + "/bot");

        } else {
          alert("Cannot add Bot player on ongoing game");
        }
    });

    $('#collectMesa').click(function() {
        grabMesaCards();
    });


    $(document).on("dragstart", "img.mycarta", function(ev) {
        var dragCard = $(ev.currentTarget);
        startDragCardToTable(dragCard);
    });

    $('#mesa').on('dragover', false).on('drop', function(ev) {
        ev.preventDefault();
        discardSelectedCard();
    });

    $(document).on("touchstart", "img.mycarta", function(ev) {
        touchInsteadOfClick = true;
        touchTargetElementTable = false;
    });


    $(document).on("touchmove", "img.mycarta", function(ev) {

        var event = ev.originalEvent;

        var dragCard = $(ev.target);
        startDragCardToTable(dragCard);

        var touchMovable = document.getElementById("touchMovable");

        if (touchMovable.classList.contains('hidden')) {
            touchMovable.classList.remove('hidden');
        }

        touchMovable.style.transform = "translateX(" + (event.touches[0].pageX) + "px) translateY(" + (event.touches[0].pageY-20) + "px) translateZ(0) scale(1)";

        var targetEl = document.elementFromPoint(event.touches[0].pageX, event.touches[0].pageY);

        touchTargetElementTable = targetEl != null &&
            (targetEl.id == "mesa" || targetEl.parentElement.id == "mesa" || targetEl.parentElement.parentElement.id == "mesa");
    });

    $(document).on("touchend", "img.mycarta", function(ev) {

        var touchMovable = document.getElementById("touchMovable");

        if (!touchMovable.classList.contains('hidden')) {
            touchMovable.classList.add('hidden');
        }

        if (touchTargetElementTable) {
            ev.preventDefault();
            discardSelectedCard();
            touchTargetElementTable = false;
        } else {
            var dragCard = $(ev.target);
            handleCardClick(dragCard);
        }
    });


});