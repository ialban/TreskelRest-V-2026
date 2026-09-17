select '0000'+b.peddScl as peddScl, '0000'+b.ventscl as ventscl, b.itemctd, b.itematv, b.itemprc, (itempto - itemprc) as itempiv, b.itempto, b.ventfch 
from( 
    select  a.peddScl, a.ventScl,  a.itemctd, a.itematv, 
            case  
                when a.ventFch < '2016/06/01' then a.itemprc / 1.12 
                when a.ventFch >= '2016/06/01' and a.ventFch < '2017/06/01' then a.itemprc / 1.14 
                when a.ventFch >= '2017/06/01' then a.itemprc / 1.12 
            end as itemprc, 
            a.itemprc as itempto, 
            a.ventFch 
    from( 
        select  p.peddScl, m.itemCtd, c.ventscl, d.itemcod, m.itemPrc, c.ventFch, m.itemAtv 
        from   tbPeddMVent d  
                inner join 
                tbPeddDVent c on c.ventScl = d.ventScl 
                inner join 
                tbPeddDPedd p on p.peddScl = c.peddScl  
                inner join 
                tbPeddMPedd m on m.peddScl = p.peddScl and m.itemCod = d.itemCod 
    ) a 
)b;

select * from tbPeddDVent

select * from tbPeddMVent