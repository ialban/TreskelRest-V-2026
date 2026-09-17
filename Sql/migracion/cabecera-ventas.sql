select '0000'+b.peddScl as peddScl, '0000'+b.ventscl as ventscl, 
		b.cajacod, b.emplcod, b.ventfch, b.itemcod, 
		b.clieNid, b.ventTip, b.ventmsa, b.ventHra, b.itemcod, b.itemprc, 
		(itempto - itemprc) as itempiv, b.itempto, b.itemAtv,
		b.ventRcb, b.ventCbo, b.ventatv, b.ventFact
from( 
    select  a.peddScl, a.ventscl, a.cajacod, a.emplcod, a.ventfch,
			a.clieNid, a.ventTip, a.ventmsa, a.ventHra, a.itemcod,
            case  
                when a.ventfch <  '2016/06/01' then a.itemprc / 1.12 
                when a.ventfch >= '2016/06/01' and a.ventfch < '2017/06/01' then a.itemprc / 1.14 
                when a.ventfch >= '2017/06/01' then a.itemprc / 1.12 
            end as itemprc, a.itemprc as itempto, a.itemAtv,
			a.ventRcb, a.ventCbo, a.ventatv, a.ventFact
    from( 
        select  p.peddScl, c.ventscl, c.cajacod, c.emplcod, c.ventfch,
				c.clieNid, c.ventTip, c.ventmsa, c.ventHra,  d.itemcod, 
				(SUM(m.itemCtd*m.itemPrc) - c.ventDsc) as itemprc, m.itemAtv,
				c.ventRcb, c.ventCbo, c.ventatv, c.ventFact
        from   tbPeddMVent d  
                inner join 
                tbPeddDVent c on c.ventScl = d.ventScl 
                inner join 
                tbPeddDPedd p on p.peddScl = c.peddScl  
                inner join 
                tbPeddMPedd m on m.peddScl = p.peddScl and m.itemCod = d.itemCod 
		group by p.peddScl, c.ventscl, c.cajacod, c.emplcod, c.ventfch,
				c.clieNid, c.ventTip, c.ventmsa, c.ventHra,  d.itemcod, 
				c.ventFch, m.itemAtv, c.ventRcb, c.ventCbo, c.ventatv, c.ventFact,
				c.ventDsc
    ) a 
)b;



--select * from tbPeddDVent

--select * from tbPeddMVent