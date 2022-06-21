const Researchers = require('./data/Researchers.json')
const HasPublications = require('./data/HasPublications.json')

const Publications = require('./data/Publications.json')
const maxEntries = 200

function search(ev) {
    if(ev.key === 'Enter') {
        fetchPublications()
    }
}

function fetchPublications() {
    clearOutput()
    document.getElementById('output').scrollTop = 0;
    const keyword = document. querySelector('input'). value
    let entriesShown = 0

    for (let i = 0; i < Publications.length; i++) {
        const matchPublication = Publications[i].Name.toLowerCase().includes(keyword.toLowerCase())
        if(matchPublication && entriesShown <= maxEntries) {
            pname = Publications[i].Name.replaceAll('"', 'hsm-pas')
            document.getElementById('output').innerHTML += `<div class="info"><span class="pubs" onclick="playBeep();fetchResearchers('${pname}')">&#8226${Publications[i].Name}</span></div>`
            entriesShown++
        }
    }

    if (document.getElementById('output').innerText == '') {
        document.getElementById('output').innerHTML+= `<div class="info"><span class="pubs">&#8226No Publications Found!</span></div>`
    } else {
        playFound()
    }
}

function fetchResearchers(publication) {
    clearOutput()
    publication = publication.replaceAll('hsm-pas', '"')
    for (let i = 0; i < HasPublications.length; i++) {
        const matchPublication = HasPublications[i].PublicationName.toLowerCase().includes(publication.toLowerCase())
        if(matchPublication) {
            const researcher = Researchers.filter(r => r.Email == HasPublications[i].Email);
            let email = HasPublications[i].Email
            if (!email.includes('@')) {
                email = 'Former employee no E-mail!'
            }

            document.getElementById('output').innerHTML+= `<div class="info" onclick="playBeep();fillCard('${researcher[0].Name}','${researcher[0].Email}','${researcher[0].Department}');showCard()">
            <span class="names">${researcher[0].Name}</span>
            <span class="emails">${email}</span></div>`
        }
    }
}

function fillCard(name, email, department) {
    document.getElementById('cdepartment').innerText = department

    document.getElementById('cname').innerText = name
    if (email.includes('@')) {
        document.getElementById('cemail').innerText = email
        document.getElementById('cemail').style.color = 'var(--primary--color-02)'
        document.querySelector('img').src = "./assets/face-icon.png";
    } else {
        document.getElementById('cemail').innerText = 'Former employee no E-mail!'
        document.getElementById('cemail').style.color = 'red'
        document.querySelector('img').src = "./assets/face-icon-crossed.png";
    }

    document.getElementById('cPubs').innerText = ''

    for (let i = 0; i < HasPublications.length; i++) {
        const matchPublication = HasPublications[i].Email.toLowerCase().includes(email.toLowerCase())
        if(matchPublication) {
            document.getElementById('cPubs').innerHTML += `<div class="pubs">&#8226${HasPublications[i].PublicationName}</div>`
        }
    }

    document.getElementById('cPubs').scrollTop = 0;
}

function playBeep() {
    const audio = document.getElementById("beepSound");
    audio.play();
}

function playFound() {
    const audio = document.getElementById("foundSound");
    audio.play();
}

function clearOutput(){
    document.getElementById('output').innerText = ''
}

function infiniteScroll(){
    // TODO
}

function hideCard(ev){
    const card = document.getElementById('card')
    if (!card.contains(ev.target)) {
        card.style.display = "none";
        document.getElementById('overlay').style.display = "none";
    }
}

function showCard(){
    document.getElementById('card').style.display = "flex";
    document.getElementById('overlay').style.display = "block";
}